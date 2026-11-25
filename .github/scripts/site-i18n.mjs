// Builds the website into _site/: the English page from html/, plus a copy of it in every
// language at /<lang>/, translated with html/i18n/<lang>.json. The tray menu mockup uses the
// app's own translations from lang/<lang>.cs, so the site and the app always say the same thing.
//
//   node .github/scripts/site-i18n.mjs           build _site/
//   node .github/scripts/site-i18n.mjs --check   only check the translations
//
// Any text that can't be found in index.html, or a translation with missing or extra keys,
// stops the build, so a stale translation can't go live.

import fs from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "../..");
const site = "https://unpluglock.com";
const htmlDir = path.join(root, "html");
const outDir = path.join(root, "_site");
const checkOnly = process.argv.includes("--check");

// app texts used on the page (the tray menu mockup), by their keys in lang/*.cs
const appKeys = [
  "Power.Online", "Menu.Enabled", "Menu.Delay", "Menu.AlsoLock", "Menu.StartupGuarded", "Menu.Guard",
  "Menu.Hide", "Menu.Password", "Menu.LockNow", "Menu.About", "Menu.Exit",
];

// lang/<file>.cs -> { key: text }
function appTable(file) {
  const source = fs.readFileSync(path.join(root, "lang", file + ".cs"), "utf8");
  const table = {};
  for (const m of source.matchAll(/\{\s*"([^"]+)",\s*"((?:[^"\\]|\\.)*)"\s*\}/g))
    table[m[1]] = JSON.parse('"' + m[2] + '"');
  return table;
}

const english = JSON.parse(fs.readFileSync(path.join(htmlDir, "i18n", "en.json"), "utf8"));
delete english._note;
const appEnglish = appTable("en");

// every language with a site translation; its app file has the same name
const languages = fs.readdirSync(path.join(htmlDir, "i18n"))
  .filter((f) => f.endsWith(".json") && f !== "en.json")
  .map((f) => f.slice(0, -5));

const pageDir = (lang) => (lang === "en" ? "" : lang.toLowerCase() + "/");
const pageUrl = (lang) => site + "/" + pageDir(lang);
const nameOf = (lang) => appTable(lang)["LanguageName"];
const pickerOrder = ["en", ...languages.sort((a, b) => nameOf(a).localeCompare(nameOf(b), "en"))];

const source = fs.readFileSync(path.join(htmlDir, "index.html"), "utf8");
const version = source.match(/"softwareVersion": "([^"]+)"/)[1];
const problems = [];

function escapeRegex(s) {
  return s.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
}

// text between tags (>text<) or a whole quoted value ("text"), so short texts like
// "Download" never match inside longer ones
function bounded(text) {
  const body = escapeRegex(text).replace(escapeRegex("{version}"), "([0-9][0-9.]*)");
  return new RegExp(`(?<=>)${body}(?=<)|(?<=")${body}(?=")`, "g");
}

function translate(lang) {
  const file = path.join(htmlDir, "i18n", lang + ".json");
  const table = JSON.parse(fs.readFileSync(file, "utf8"));
  const app = appTable(lang);

  for (const key of Object.keys(english))
    if (!(key in table)) problems.push(`${lang}.json is missing ${key}`);
  for (const key of Object.keys(table))
    if (!(key in english)) problems.push(`${lang}.json has ${key}, which en.json doesn't`);

  const fill = (text) => text.replace(/\{app:([\w.]+)\}/g, (_, k) => {
    if (!(k in app)) problems.push(`${lang}.json uses {app:${k}}, which lang/${lang}.cs doesn't have`);
    return app[k];
  });

  let html = source;
  // longest first, so a text is replaced before any shorter text inside it
  const pairs = Object.keys(english)
    .filter((k) => !k.startsWith("lang.")) // used by the picker, not found in the page
    .map((k) => [english[k], table[k] === undefined ? english[k] : fill(table[k])])
    .concat(appKeys.map((k) => [appEnglish[k], app[k]]))
    .sort((a, b) => b[0].length - a[0].length);
  for (const [from, to] of pairs) {
    const pattern = bounded(from);
    if (!pattern.test(html)) {
      problems.push(`index.html no longer contains: ${from}`);
      continue;
    }
    html = html.replace(bounded(from), (match, ...groups) => {
      const quoted = html[groups[groups.length - 2] - 1] === '"';
      if (quoted && to.includes('"')) problems.push(`${lang}: a translation used in an attribute can't contain "  (${to})`);
      const captured = groups.slice(0, -2).find((g) => typeof g === "string");
      return to.replace("{version}", captured || version);
    });
  }
  return html;
}

// the bits that differ per page but aren't text: lang, URLs, the picker, structured data
function finish(html, lang) {
  const t = lang === "en" ? english : JSON.parse(fs.readFileSync(path.join(htmlDir, "i18n", lang + ".json"), "utf8"));
  const up = lang === "en" ? "" : "../";

  html = html.replace('<html lang="en">', `<html lang="${lang}">`);
  html = html.replace(/<link rel="canonical" href="[^"]*">/, () =>
    `<link rel="canonical" href="${pageUrl(lang)}">\n` +
    pickerOrder.map((l) => `  <link rel="alternate" hreflang="${l}" href="${pageUrl(l)}">`).join("\n") +
    `\n  <link rel="alternate" hreflang="x-default" href="${pageUrl("en")}">`);
  html = html.replace(/<meta property="og:url" content="[^"]*">/, `<meta property="og:url" content="${pageUrl(lang)}">`);
  html = html.replace('<a class="brand" href="/">', `<a class="brand" href="/${pageDir(lang)}">`);
  if (up)
    html = html.replace(/(href|src)="(?!https?:|\/|#|data:|mailto:)([^"]+)"/g, `$1="${up}$2"`);

  const flag = (l) => `<img src="${up}img/flags/${l}.svg" alt="" width="21" height="14">`;
  const items = pickerOrder.map((l) =>
    `          <li><a href="/${pageDir(l)}" hreflang="${l}" lang="${l}"${l === lang ? ' aria-current="page"' : ""}` +
    ` data-umami-event="language" data-umami-event-lang="${l}">${flag(l)}${nameOf(l)}</a></li>`);
  const code = lang.split("-")[0].toUpperCase();
  const picker =
    `<details class="lang">\n` +
    `        <summary aria-label="${t["lang.label"]}: ${nameOf(lang)}">${flag(lang)}<span>${code}</span></summary>\n` +
    `        <ul>\n${items.join("\n")}\n        </ul>\n      </details>`;
  html = html.replace(/<details class="lang">[\s\S]*?<\/details>/, picker);

  // structured data: this page's URL and language, and the questions as they read on this page
  html = html.replace(/(<script type="application\/ld\+json">)([\s\S]*?)(<\/script>)/, (_, open, json, close) => {
    const data = JSON.parse(json);
    const app = data["@graph"].find((g) => g["@type"] === "SoftwareApplication");
    app.url = pageUrl(lang);
    app.inLanguage = lang;
    const faq = data["@graph"].find((g) => g["@type"] === "FAQPage");
    faq.inLanguage = lang;
    faq.mainEntity = [...html.matchAll(/<details>\s*<summary[^>]*>([\s\S]*?)<\/summary>\s*<p>([\s\S]*?)<\/p>/g)].map((m) => ({
      "@type": "Question",
      name: plain(m[1]),
      acceptedAnswer: { "@type": "Answer", text: plain(m[2]) },
    }));
    return open + "\n  " + JSON.stringify(data, null, 2).replace(/\n/g, "\n  ") + "\n  " + close;
  });
  return html;
}

function plain(html) {
  const entities = { amp: "&", rarr: "→", ndash: "–", middot: "·", nbsp: " ", quot: '"', lt: "<", gt: ">" };
  return html.replace(/<[^>]+>/g, "").replace(/&(\w+);/g, (m, e) => entities[e] ?? m).replace(/\s+/g, " ").trim();
}

const pages = { en: finish(source, "en") };
for (const lang of languages)
  pages[lang] = finish(translate(lang), lang);

if (problems.length) {
  console.error([...new Set(problems)].join("\n"));
  process.exit(1);
}

if (checkOnly) {
  console.log(`translations ok: ${languages.length} languages`);
  process.exit(0);
}

fs.rmSync(outDir, { recursive: true, force: true });
fs.cpSync(htmlDir, outDir, { recursive: true, filter: (p) => !p.startsWith(path.join(htmlDir, "i18n")) });
for (const [lang, html] of Object.entries(pages)) {
  fs.mkdirSync(path.join(outDir, pageDir(lang)), { recursive: true });
  fs.writeFileSync(path.join(outDir, pageDir(lang), "index.html"), html);
}

// sitemap with every language, each pointing at the others
const today = new Date().toISOString().slice(0, 10);
const alternates = pickerOrder.map((l) => `    <xhtml:link rel="alternate" hreflang="${l}" href="${pageUrl(l)}"/>`).join("\n");
fs.writeFileSync(path.join(outDir, "sitemap.xml"),
  `<?xml version="1.0" encoding="UTF-8"?>\n` +
  `<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9" xmlns:xhtml="http://www.w3.org/1999/xhtml">\n` +
  pickerOrder.map((l) => `  <url>\n    <loc>${pageUrl(l)}</loc>\n    <lastmod>${today}</lastmod>\n${alternates}\n  </url>`).join("\n") +
  `\n</urlset>\n`);

console.log(`built ${outDir}: ${pickerOrder.length} languages`);

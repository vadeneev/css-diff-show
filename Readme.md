# CSS difference


If you need to compare/refactor/move legacy css to existing preprocessor generated styles or compare two style files difference - this tool wsill help you and will try to save your time.

It will generate file looks like:
```
.mystyle {
-  padding: 3em;
+  padding: 5em;
}
// ----------------- this style must be removed from first file
.mystyle-style-from-first-file {
  padding: 5em;
}
// ---------------------
// +++++++++++++++++++++ this style must be added to first file
.mystyle-style-from-second-file {
  padding: 5em;
}
// +++++++++++++++++++++
```
# .net Build with VS 2017
  * supports @media
  * normalizing css files to compare
  * used HASH for fast search (and same selectors puts to one container for comparing)
  * sort multiple selectors (no ordering problems)
  * Checks same rule values. ex: color: white; is same as color: #fff; or #ffffff;

 To Do:
 *  support @ - selector-rules (keyframes etc.)
 *  imporve same-rule-values check
 *  create LESS/SASS/STYLUS generator module
  

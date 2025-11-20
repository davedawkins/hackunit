import fs from 'node:fs';

export function getFile( path ) { return fs.readFileSync(path, 'utf8'); }
const fs = require('fs');
const path = require('path');
const sharp = require('sharp');

// Config
const samplesDir = path.resolve(__dirname, '../src/assets/samples');
const outBase = path.resolve(samplesDir, 'resized');
const args = process.argv.slice(2);
const sizes = args.length > 0 ? args.map(s => parseInt(s, 10)).filter(Boolean) : [512, 256, 128];

async function ensureDir(dir) {
  await fs.promises.mkdir(dir, { recursive: true });
}

async function resizeFile(filePath, size) {
  const fileName = path.basename(filePath);
  const outDir = path.join(outBase, String(size));
  await ensureDir(outDir);
  const outPath = path.join(outDir, fileName);
  await sharp(filePath)
    .resize(size, size, { fit: 'cover' })
    .png({ quality: 90 })
    .toFile(outPath);
  return outPath;
}

async function main() {
  console.log(`Resizing images in ${samplesDir} to sizes: ${sizes.join(', ')}`);
  const entries = await fs.promises.readdir(samplesDir, { withFileTypes: true });
  const files = entries
    .filter(e => e.isFile())
    .map(e => e.name)
    .filter(n => /\.(png|jpg|jpeg)$/i.test(n))
    .map(n => path.join(samplesDir, n));

  if (files.length === 0) {
    console.log('No image files found in samples folder.');
    return;
  }

  for (const size of sizes) {
    console.log(`\nProcessing size ${size}...`);
    for (const f of files) {
      try {
        const out = await resizeFile(f, size);
        console.log('Wrote', out);
      } catch (err) {
        console.error('Error processing', f, '->', err.message);
      }
    }
  }

  console.log('\nDone. Resized images are in:', outBase);
}

main().catch(err => {
  console.error(err);
  process.exit(1);
});

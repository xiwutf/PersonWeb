const fs = require('fs');
const path = require('path');
const { promisify } = require('util');
const sharp = require('sharp');
const glob = require('glob');

const readFile = promisify(fs.readFile);
const writeFile = promisify(fs.writeFile);
const mkdir = promisify(fs.mkdir);

// 图片配置
const imageConfig = {
  // 输入目录
  inputDir: path.join(__dirname, '../public/images'),
  // 输出目录
  outputDir: path.join(__dirname, '../public/images/optimized'),
  // 优化配置
  formats: [
    { format: 'webp', quality: 80 },
    { format: 'jpeg', quality: 80 },
    { format: 'png', quality: 80 }
  ],
  // 最大尺寸
  maxWidth: 1920,
  maxHeight: 1080,
  // 忽略的文件
  ignore: ['README.md', '.gitkeep']
};

// 图片需要压缩的文件列表
const imagesToOptimize = [
  'avatar.jpg',
  'wechat-qr.png',
  'blog/thermal-circulation.png'
];

async function optimizeImages() {
  try {
    // 创建输出目录
    await mkdir(imageConfig.outputDir, { recursive: true });

    for (const imageFile of imagesToOptimize) {
      if (imageConfig.ignore.includes(imageFile)) continue;

      const inputPath = path.join(imageConfig.inputDir, imageFile);
      const fileName = path.basename(imageFile, path.extname(imageFile));
      const fileExt = path.extname(imageFile);

      if (!fs.existsSync(inputPath)) {
        console.log(`文件不存在: ${inputPath}`);
        continue;
      }

      console.log(`正在优化: ${imageFile}`);

      // 获取图片信息
      const metadata = await sharp(inputPath).metadata();

      // 对每种格式进行优化
      for (const { format, quality } of imageConfig.formats) {
        // 跳过原始格式与目标格式相同的 JPEG/PNG
        if ((fileExt === '.jpg' || fileExt === '.jpeg') && format === 'jpeg') continue;
        if (fileExt === '.png' && format === 'png') continue;

        const outputPath = path.join(
          imageConfig.outputDir,
          `${fileName}.${format}`
        );

        // 调整尺寸
        let transformer = sharp(inputPath);

        if (metadata.width > imageConfig.maxWidth ||
            metadata.height > imageConfig.maxHeight) {
          transformer = transformer.resize(
            imageConfig.maxWidth,
            imageConfig.maxHeight,
            { fit: 'inside', withoutEnlargement: true }
          );
        }

        // 转换和压缩
        transformer = transformer.toFormat(format, { quality });

        // 特殊处理 WebP
        if (format === 'webp') {
          transformer = transformer.webp({ quality, effort: 6 });
        }

        await transformer.toFile(outputPath);
        console.log(`已生成: ${path.basename(outputPath)}`);
      }
    }

    console.log('\n图片优化完成！');
    console.log(`优化后的图片保存在: ${imageConfig.outputDir}`);

  } catch (error) {
    console.error('图片优化错误:', error);
  }
}

// 检查是否安装了 sharp
if (require.main === module) {
  // 检查依赖
  if (!fs.existsSync('node_modules/sharp')) {
    console.error('错误: 请先安装 sharp');
    console.log('运行: npm install --save-dev sharp');
    process.exit(1);
  }
  optimizeImages();
}

module.exports = { optimizeImages };
const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CssMinimizerPlugin = require('css-minimizer-webpack-plugin');
const TerserPlugin = require('terser-webpack-plugin');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const fs = require('fs');

const deleteFilesByPattern = (directory, pattern) => {
    if (!fs.existsSync(directory)) return;
    const files = fs.readdirSync(directory);
    const regex = new RegExp(pattern);
    files.forEach(file => {
        if (regex.test(file)) {
            fs.unlinkSync(path.join(directory, file));
        }
    });
};

deleteFilesByPattern(path.resolve(__dirname, 'Areas/VA011/Scripts/dist'), /^VA011\.all\.min(\.\d+\.\d+\.\d+\.\d+)?\.js$/);
deleteFilesByPattern(path.resolve(__dirname, 'Areas/VA011/Scripts/dist'), /^VA011React\.min(\.\d+\.\d+\.\d+\.\d+)?\.js$/);
deleteFilesByPattern(path.resolve(__dirname, 'Areas/VA011/Contents'), /^VA011\.all\.min(\.\d+\.\d+\.\d+\.\d+)?\.css$/);

const versions = {
    'VA011.all': '1.0.0.0',   // JS version
    'VA011React': '1.0.0.0',  // React version
    'VA011': '1.0.0.0'        // CSS version
};

module.exports = {
    mode: 'development',
    //mode: 'production',
    devtool: false,
    entry: {
        'VA011.all': './Areas/VA011/Scripts/src/VA011js.js',
        'VA011React': './Areas/VA011/Scripts/src/VA011reactjs.js',
        'VA011': './Areas/VA011/Contents/src/VA011css.css'
    },
    output: {
        filename: ({ chunk }) => {
            const name = chunk.name;
            const version = versions[name] || '1.0.0.0';
            return `${name}.min.js`;
        },
        path: path.resolve(__dirname, 'Areas/VA011/Scripts/dist')
    },
    resolve: {
        extensions: ['.jsx', '.js'],
    },
    module: {
        rules: [
            {
                test: /\.(js|jsx)$/,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader'
                },
            },
            {
                test: /\.(css|sass)$/,
                use: [MiniCssExtractPlugin.loader, {
                    loader: 'css-loader',
                    options: {
                        url: false,
                    }
                }]
            }
        ]
    },
    plugins: [
        new CleanWebpackPlugin(),
        new MiniCssExtractPlugin({
            filename: ({ chunk }) => {
                const name = chunk.name;
                return `../../Contents/${name}.all.min.css`;
            },
        }),
    ],
    optimization: {
        minimize: true,
        minimizer: [
            new CssMinimizerPlugin(),
            new TerserPlugin({
                terserOptions: {
                    compress: {
                        drop_debugger: true,
                        drop_console: false,
                    },
                    format: {
                        comments: false,
                    },
                },
                extractComments: false,
            }),
        ]
    },
};

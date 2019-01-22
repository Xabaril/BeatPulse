const webpack = require('webpack');
const path = require('path');

module.exports = {
    devtool: 'none',
    entry: path.resolve(__dirname, "../Client/index.tsx"),
    resolve: {
        extensions: [".tsx",".ts", ".json"]
    },
    output: {
        path: path.join(__dirname, "../Assets"),
        filename: 'beatpulse-bundle.js'
    },
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                loader: "awesome-typescript-loader",
                exclude: [/(node_modules)/]
            },
            {
                loader: 'url-loader',
                test: /\.(png|jpg|gif|svg)$/                
            }
        ]
    },
    plugins: [
        new webpack.DllReferencePlugin({
            context: ".",
            manifest: require("../Assets/vendors-manifest.json")
        })
    ]
};
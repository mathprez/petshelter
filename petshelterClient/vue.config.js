module.exports = {
    configureWebpack: {
        devtool: 'source-map',
    },
    devServer: {
        proxy: {
            "/api": {
                target: "https://localhost:44385"
            }
        }
    }
};
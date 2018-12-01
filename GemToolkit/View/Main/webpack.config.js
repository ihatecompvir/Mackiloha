var path = require('path')
var webpack = require('webpack')
var utils = require('./utils')

const MiniCssExtractPlugin = require("mini-css-extract-plugin")
const OptimizeCssAssetsPlugin = require('optimize-css-assets-webpack-plugin')
const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin

const UglifyJsPlugin = require('uglifyjs-webpack-plugin')
const VueLoaderPlugin = require('vue-loader/lib/plugin')
const VuetifyLoaderPlugin = require('vuetify-loader/lib/plugin')

const analyze = process.env.ANALYZE === true || process.env.ANALYZE === 'true'

var output = {
  path: path.resolve(__dirname, './dist'),
  filename: '[name].js'
}

var resolve = (p) => path.resolve(__dirname, p)

var webpackOptions = {
  mode: 'development',
  output: output,
  module: {
    rules: [
      {
        test: /\.vue$/,
        loader: 'vue-loader'
      },
      {
        test: /\.js$/,
        loader: 'babel-loader',
        exclude: /node_modules/
      },
      {
        test: /\.(png|jpe?g|gif|svg)(\?.*)?$/,
        loader: 'file-loader',
        options: {
          name: '[name].[ext]?[hash]'
        }
      },
      {
        // Match woff2 in addition to patterns like .woff?v=1.1.1.
        test: /\.(woff|woff2|ttf|eot)(\?v=\d+\.\d+\.\d+)?$/,
        loader: "file-loader",
        options: {
          limit: 50000,
          name: './fonts/[name].[ext]'
        }
      },
      {
        test: /\.json$/,
        loader: 'json-loader'
      },
      {
        test: /\.cjson$/,
        loader: 'raw-loader'
      },
      {
        test: /\.styl(us)?$/,
        loader: ['style-loader', 'css-loader', 'stylus-loader']
      },
      {
        test: /\.html$/,
        loader: 'html-loader'
      }
    ]
  },
  devServer: {
    historyApiFallback: true,
    noInfo: true,
    overlay: true
  },
  resolve: {
    extensions: ['.js', '.vue', '.json', '.css', '.cjson'],
    alias: {
      'src': path.resolve(__dirname, '../src'),
      'assets': path.resolve(__dirname, '../src/assets'),
      'components': path.resolve(__dirname, '../src/components')
    }
  },
  plugins: [
    new VueLoaderPlugin(),
    new VuetifyLoaderPlugin()
  ],
  devtool: 'source-map',
  optimization: {
    concatenateModules: true,
    splitChunks: {
      cacheGroups: {
        default: false,
        vendors: false
      }
    }
  }
}

var buildMode = false
switch (process.env.NODE_ENV) {
  case 'production':
    webpackOptions.mode = 'production'

    buildMode = true
    webpackOptions.externals = {
      'vue': 'Vue',
      'vueHelper': 'glueHelper'
    }
    webpackOptions.entry = { main: './src/entry.js' }

    webpackOptions.devtool = '#cheap-source-map'
    // http://vue-loader.vuejs.org/en/workflow/production.html
    webpackOptions.plugins = (webpackOptions.plugins || []).concat([
      new webpack.DefinePlugin({
        'process.env': {
          NODE_ENV: '"production"'
        }
      }),
      new webpack.optimize.ModuleConcatenationPlugin(),
      new MiniCssExtractPlugin({
        filename: '[name].css',
        chunkFilename: '[name].css'
      }),
      new webpack.LoaderOptionsPlugin({
        minimize: true
      })
    ])
    break

  case 'development':
    webpackOptions.plugins = (webpackOptions.plugins || []).concat([
      new webpack.HotModuleReplacementPlugin()
    ])

    webpackOptions.resolve.alias = {
      'vue$': 'vue/dist/vue'
    }
    webpackOptions.entry = { main: './src/main.js' }
    break
  case 'integrated':
    webpackOptions.plugins = (webpackOptions.plugins || []).concat([
      new webpack.HotModuleReplacementPlugin()
    ])

    webpackOptions.externals = {
      'vue': 'Vue',
      'vueHelper': 'glueHelper'
    }
    webpackOptions.entry = { main: './src/integrated.js' }
    break
}

const styleOption = buildMode ? { sourceMap: true, extract: true } : { sourceMap: true }
webpackOptions.module.rules = webpackOptions.module.rules.concat(utils.styleLoaders(styleOption))

if (analyze) {
  webpackOptions.plugins = (webpackOptions.plugins || []).concat([
    new BundleAnalyzerPlugin()
  ])
}

module.exports = webpackOptions

// Vuetify
import 'vuetify/dist/vuetify.min.css'
import colors from 'vuetify/es5/util/colors'
import 'material-design-icons-iconfont/dist/material-design-icons.css'
import Vuetify from 'vuetify/lib'

function install(Vue) {
  // Call vue use here if needed
  Vue.use(Vuetify)
}

function vueInstanceOption() {
  // Return vue global option here, such as vue-router, vue-i18n, mix-ins, .... 
  return {}
}

export {
    install,
    vueInstanceOption
} 

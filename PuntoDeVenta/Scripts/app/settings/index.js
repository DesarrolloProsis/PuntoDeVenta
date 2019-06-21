import Vue from 'vue'
import BootstrapVue from 'bootstrap-vue'

Vue.use(BootstrapVue)

import 'bootstrap-vue/dist/bootstrap-vue.css'
import App from './Home.vue'

Vue.config.productionTip = false

new Vue({
    render: h => h(App)
}).$mount('#config')
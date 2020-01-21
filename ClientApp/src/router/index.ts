import Vue from 'vue';
import VueRouter from 'vue-router';
import Home from '../views/Home.vue';
import Gallery from '@/views/Gallery.vue';

Vue.use(VueRouter);

const routes = [
  {
    path: '/',
    name: 'home',
    component: Home,
  },
  {
    path: '/about',
    name: 'about',
    component: () => import('../views/About.vue'),
  },
  {
    path: '/gallery/:pageNum',
    name: 'gallery',
    component: Gallery,
  }, {
    path: '/uploadImage',
    name: 'uploadImage',
    component: () => import('../views/UploadImage.vue'),
  },
];

const router = new VueRouter({
  mode: 'history',
  routes,
});

export default router;

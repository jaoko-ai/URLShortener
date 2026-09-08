import { createMemoryHistory, createRouter } from "vue-router";

const routes = [
  {
    path: "/",
    name: "Home View",
    component: () => import("../views/HomeView.vue"),
  },
  {
    path: "/about",
    name: "about Page",
    component: () => import("../views/AboutView.vue"),
  },
];

export const router = createRouter({
  history: createMemoryHistory(),
  routes,
});

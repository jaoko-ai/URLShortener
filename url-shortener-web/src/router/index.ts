import { createRouter, createWebHistory } from "vue-router";
import { useAuthStore } from "../stores/auth";
import "./types";

export const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: "/",
      name: "home",
      component: () => import("../views/HomeView.vue"),
    },
    {
      path: "/login",
      name: "login",
      component: () => import("../views/AuthView.vue"),
      meta: { guestOnly: true, mode: "login" },
    },
    {
      path: "/register",
      name: "register",
      component: () => import("../views/AuthView.vue"),
      meta: { guestOnly: true, mode: "register" },
    },
    {
      path: "/about",
      name: "about",
      component: () => import("../views/AboutView.vue"),
    },
    {
      path: "/:pathMatch(.*)*",
      name: "not-found",
      component: () => import("../views/NotFoundView.vue"),
    },
  ],
  scrollBehavior() {
    return { top: 0 };
  },
});

router.beforeEach((to) => {
  const auth = useAuthStore();
  if (to.meta.guestOnly && auth.isAuthenticated) {
    return { name: "home" };
  }
  return true;
});

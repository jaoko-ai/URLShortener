export {};

declare module "vue-router" {
  interface RouteMeta {
    guestOnly?: boolean;
    mode?: "login" | "register";
  }
}

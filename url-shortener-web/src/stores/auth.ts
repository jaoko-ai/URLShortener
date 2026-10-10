import { defineStore } from "pinia";
import { computed, ref } from "vue";
import * as authApi from "../api/auth";
import { getErrorMessage, getStoredToken, setStoredToken } from "../api/client";
import type { LoginRequest, RegisterRequest, User } from "../api/types";

export const useAuthStore = defineStore("auth", () => {
  const user = ref<User | null>(null);
  const token = ref<string | null>(getStoredToken());
  const loading = ref(false);
  const error = ref("");

  const isAuthenticated = computed(() => Boolean(token.value && user.value));

  function setSession(nextToken: string, nextUser: User) {
    token.value = nextToken;
    user.value = nextUser;
    setStoredToken(nextToken);
  }

  function clearSession() {
    token.value = null;
    user.value = null;
    setStoredToken(null);
  }

  async function register(payload: RegisterRequest) {
    loading.value = true;
    error.value = "";
    try {
      const response = await authApi.register(payload);
      setSession(response.token, response.user);
    } catch (err) {
      error.value = getErrorMessage(err);
      throw err;
    } finally {
      loading.value = false;
    }
  }

  async function login(payload: LoginRequest) {
    loading.value = true;
    error.value = "";
    try {
      const response = await authApi.login(payload);
      setSession(response.token, response.user);
    } catch (err) {
      error.value = getErrorMessage(err);
      throw err;
    } finally {
      loading.value = false;
    }
  }

  async function logout() {
    try {
      if (token.value) {
        await authApi.logout();
      }
    } catch {
      // Session is cleared locally even if the API call fails.
    } finally {
      clearSession();
    }
  }

  async function hydrate() {
    if (!token.value) return;
    try {
      user.value = await authApi.getMe();
    } catch {
      clearSession();
    }
  }

  return {
    user,
    token,
    loading,
    error,
    isAuthenticated,
    register,
    login,
    logout,
    hydrate,
    clearSession,
  };
});

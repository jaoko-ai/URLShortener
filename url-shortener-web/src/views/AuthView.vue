<script setup lang="ts">
import { ref } from "vue";
// const message = ref("");
const method = ref("login");

const changeMethod = () => {
  if (method.value == "login") {
    method.value = "register";
  } else {
    method.value = "login";
  }
};

const emailInput = ref("");
const passwordInput = ref("");
const usernameInput = ref("");

const fromSumbmit = () => {
  // alert("submitting");
};
</script>
<template>
  <div>
    <div
      v-show="method == 'register'"
      class="flex w-full items-center gap-3 p-3 pt-1"
    >
      <label for="usernameInput">Username: </label>
      <input
        id="usernameInput"
        class="border rounded-sm w-full p-2"
        required
        type="email"
        placeholder="Enter your email"
        v-model="usernameInput"
      />
    </div>

    <div class="flex w-full items-center gap-3 p-3 pt-1">
      <label for="emailInput">Email: </label>
      <input
        id="emailInput"
        class="border rounded-sm w-full p-2"
        required
        type="email"
        placeholder="Enter your email"
        v-model="emailInput"
      />
    </div>
    <div class="flex items-center gap-2 p-3 pt-1">
      <label for="passwordInput">Password: </label>
      <input
        id="passwordInput"
        class="border rounded-sm w-full p-2"
        type="password"
        required
        placeholder="Enter your secret password"
        v-model="passwordInput"
      />
    </div>
    <button
      class="w-full p-1 m-5 text-2xl bg-blue-300 hover:bg-blue-400 active:bg-blue-500 rounded-2xl transition-colors ease-initial"
      @click="fromSumbmit"
      type="submit"
    >
      {{ method }}
    </button>
    <p>
      Have an acc already:
      <button class="text-blue-400" @click="changeMethod">
        {{ method }}
      </button>
    </p>
  </div>
</template>

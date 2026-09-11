<script setup lang="ts">
import { ref } from "vue";
// const message = ref("");
const method = ref("login");
</script>
<template>
  <div>
    <input type="email" />
    <input type="password" />
    <button type="submit">{{ method }}</button>
    <p>Have an acc already: login</p>
  </div>
</template>

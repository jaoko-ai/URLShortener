<script setup lang="ts">
import { ref } from "vue";
import axios from "axios";
const backendURL: string = "http://localhost:5062/";
const path = "shorten";
const urlInput = ref("");
const errorMessage = ref("");
let URLResponse = ref("");
const urlRegex =
  /^https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)$/;

const PostShorten = () => {
  errorMessage.value = "";
  if (urlInput.value === null || urlInput.value === "") {
    errorMessage.value = "cannot be empty";
    return;
  }
  if (!urlRegex.test(urlInput.value.trim())) {
    errorMessage.value = "Enter a valid url";
    return;
  }

  axios
    .post(backendURL + path, {
      LongUrl: urlInput.value,
    })
    .then((response) => {
      URLResponse.value = response.data.shortUrl;
      console.log(response.data);
    })
    .catch((error) => {
      errorMessage.value = error;
      console.log(error);
    })
    .finally(() => {
      urlInput.value = "";
    });
};
</script>
<template>
  <div class="flex justify-around gap-3 items-center">
    <div class="border rounded-xl p-5 max-h-50 flex-1">
      <h3 class="block">Shorten a URL</h3>
      <form @submit.prevent="PostShorten" id="ShortenUrlInput">
        <label for="UrlInput">Enter a url: </label>
        <input
          id="UrlInput"
          type="text"
          class="border border-purle-300"
          placeholder="Enter a URL"
          v-model="urlInput"
        />
        <button
          class="border p-1 m-1 rounded-full bg-blue-500 hover:bg-blue-700 hover:shadow-xl active:bg-blue-600 hover:cursor-pointer"
          type="submit"
        >
          shorten
        </button>
        <p class="text-red-600 text-xl">{{ errorMessage }}</p>
      </form>
      <a :href="URLResponse" target="_blank">ShortCode: {{ URLResponse }}</a>
    </div>
    <div class="border min-w-100 rounded-xl p-5 min-h-100 shrink-0">
      <div><h1>Links</h1></div>
      <div>
        <h1>Clicks Previews</h1>
      </div>
    </div>
  </div>
  <div class="border rounded-xl">
    <h1>Pie hole data representation</h1>
  </div>
</template>

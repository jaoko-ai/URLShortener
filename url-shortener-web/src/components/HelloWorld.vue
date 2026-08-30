<script setup lang="ts">
import { ref } from "vue";
import axios from "axios";

const MakeGet = async () => {
	const response = await axios.get("http://localhost:5062/2j", {
		timeout: 1000,
	});
};

let urlInput = ref("");

function PostRequest() {
	console.error(`User input: ${urlInput.value}`);
	urlInput.value = "";
}
</script>

<template>
	<div class="">
		<div>
			<Label for="urlShortenerInput"> Enter a URL to shorten: </Label>
			<input
				type="text"
				v-model="urlInput"
				placeholder="Enter a URL to shorten"
			/>

			<button @click="PostRequest">submit</button>
		</div>
	</div>

	<div>
		<p>Live display: {{ urlInput }}</p>
	</div>
</template>

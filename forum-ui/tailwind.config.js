/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{js,jsx,ts,tsx}",
    "node_modules/flowbite-react/**/*.{js,jsx,ts,tsx}",
  ],
  theme: {
    extend: {
      spacing: {
        128: "32rem",
        144: "48rem",
      },
    },
  },
  plugins: [require("flowbite/plugin"), require("@tailwindcss/line-clamp")],
};

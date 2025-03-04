// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

valueShown = document.querySelector("#valueShown");
price = document.querySelector("#price").value;
price50 = document.querySelector("#price50").value;
price100 = document.querySelector("#price100").value;
countProducts = document.querySelector("#countProducts").value;
console.log(`${valueShown}`)

if (countProducts <= 50) {
    valueShown.innerText = (parseFloat(price) * parseInt(countProducts)).toString();
}
else if (countProducts <= 100) {
    valueShown.innerText = (parseFloat(price50) * parseInt(countProducts)).toString();
}
else {
    valueShown.innerText = (parseFloat(price100) * parseInt(countProducts)).toString();
}
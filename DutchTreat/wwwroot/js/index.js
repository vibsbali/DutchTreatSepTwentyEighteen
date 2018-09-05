"use strict";

$(document).ready(function() {
   console.log("running index.js");
   var theform = $("#theForm");
   theform.hide();

   var button = $("#buyButton");
   button.on("click", function () {
      console.log("Buying Item");
   });

   var productInfo = $(".product-props li");
   productInfo.on("click",
      function () {
         console.log("You clicked on " + $(this).text());
      });


   var $loginToggle = $("#loginToggle");
   var $popupForm = $(".popup-form");

   $loginToggle.on("click",
      function() {
         $popupForm.slideToggle(1000);
      });
});
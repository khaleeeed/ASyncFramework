/* Custom JavaScript */
$(document).ready(function($) {
	
	/*---------- For Placeholder on IE9 and below -------------*/
	$('input, textarea').placeholder();
	
	/*----------- For icon rotation on input box foxus -------------------*/ 	
	$('.input-field').focus(function() {
  		$('.page-icon img').addClass('rotate-icon');
	});
	
	/*----------- For icon rotation on input box blur -------------------*/ 	
	$('.input-field').blur(function() {
  		$('.page-icon img').removeClass('rotate-icon');
	});
	
});


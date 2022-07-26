 /*-----------------------------------------------------------------------------------
/* Custom Scripts
-----------------------------------------------------------------------------------*/

/* ----------------- Start Document ----------------- */
(function($){
	$(document).ready(function(){
	
/*----------------------------------------------------*/
/*	Navigation
/*----------------------------------------------------*/

	$(".menu li").hover(
			function () {
				$(this).find('ul:first').css({
					visibility: "visible",
					display: "none"
				}).stop(true, true).fadeIn(100);
			},
			function () {
				$(this).find('ul:first').css({
					visibility: "visible",
					display: "block"
				}).stop(true, true).fadeOut(100);
			}
	);
			
	selectnav('responsive', {
		label: 'القائمة',
		nested: true,
		indent: '&nbsp;&nbsp;&nbsp;'
	});

/*----------------------------------------------------*/
/*	drop login
/*----------------------------------------------------*/

/*	
  $(".logbar").hide();
  $(".logbut").addClass("plus").show();
  $('.logbut').toggle(
      function(){
          $(".logbar").slideDown().slideToggle("fast");
          $(this).addClass("plus");
          $(this).removeClass("minus");
      },
      function(){
          $(".logbar").slideUp().slideToggle("fast");
          $(this).addClass("minus");
          $(this).removeClass("plus");
      }
  );

*/

/*----------------------------------------------------*/
/*	Carousel
/*----------------------------------------------------*/

// Add classes for other carousels
var $carousel = $('.recent-blog-jc, .recent-work-jc');

var scrollCount;

function adjustScrollCount() {
	if( $(window).width() < 768 ) {
		scrollCount = 1;
	} else {
		scrollCount = 3;
	}

}

function adjustCarouselHeight() {

	$carousel.each(function() {
		var $this    = $(this);
		var maxHeight = -1;
		$this.find('li').each(function() {
			maxHeight = maxHeight > $(this).height() ? maxHeight : $(this).height();
		});
		$this.height(maxHeight);
	});
}
function initCarousel() {
	adjustCarouselHeight();
	adjustScrollCount();
	var i = 0;
	var g = {};
	$carousel.each(function() {
		i++;

		var $this = $(this);
		g[i] = $this.jcarousel({
			animation           : 600,
			scroll              : scrollCount
		});
		$this.jcarousel('scroll', 0);
		 $this.prev().find('.jcarousel-prev').bind('active.jcarouselcontrol', function() {
			$(this).addClass('active');
		}).bind('inactive.jcarouselcontrol', function() {
			$(this).removeClass('active');
		}).jcarouselControl({
			target: '-='+scrollCount,
			carousel: g[i]
		});

		$this.prev().find('.jcarousel-next').bind('active.jcarouselcontrol', function() {
			$(this).addClass('active');
		}).bind('inactive.jcarouselcontrol', function() {
			$(this).removeClass('active');
		}).jcarouselControl({
			target: '+='+scrollCount,
			carousel: g[i]
		});

		$this.touchwipe({
		wipeLeft: function() {
			$this.jcarousel('scroll','+='+scrollCount);
		},
		wipeRight: function() {
			$this.jcarousel('scroll','-='+scrollCount);
		}
	});

	});
}
$(window).load(function(){
	initCarousel();
});

$(window).resize(function () {
	$carousel.each(function() {
		var $this = $(this);
		$this.jcarousel('destroy');
	});
	initCarousel();
});


/*----------------------------------------------------*/
/*	Alert Boxes
/*----------------------------------------------------*/

	$(document.body).pixusNotifications({
		speed: 300,
		animation: 'fadeAndSlide',
		hideBoxes: false
	});


/*----------------------------------------------------*/
/*	Tabs
/*----------------------------------------------------*/

	var $tabsNav    = $('.tabs-nav'),
		$tabsNavLis = $tabsNav.children('li'),
		$tabContent = $('.tab-content');

	$tabsNav.each(function() {
		var $this = $(this);

		$this.next().children('.tab-content').stop(true,true).hide()
											 .first().show();

		$this.children('li').first().addClass('active').stop(true,true).show();
	});

	$tabsNavLis.on('click', function(e) {
		var $this = $(this);

		$this.siblings().removeClass('active').end()
			 .addClass('active');

		$this.parent().next().children('.tab-content').stop(true,true).hide()
													  .siblings( $this.find('a').attr('href') ).fadeIn();

		e.preventDefault();
	});


/*----------------------------------------------------*/
/*	Accordion
/*----------------------------------------------------*/

	$( "#accordion" ).accordion({
		heightStyle: "content"
	});
	var icons = {
		header: "ui-accordion-icon",
		activeHeader: "ui-accordion-icon-active"
	};
	$( "#accordion" ).accordion({
		icons: icons
	});
	$( "#toggle" ).button().click(function() {
		if ( $( "#accordion" ).accordion( "option", "icons" ) ) {
			$( "#accordion" ).accordion( "option", "icons", null );
		} else {
			$( "#accordion" ).accordion( "option", "icons", icons );
		}
	});


/*----------------------------------------------------*/
/*	Toggle
/*----------------------------------------------------*/

	$(".toggle-container").hide();
	$(".trigger").toggle(function(){
		$(this).addClass("active");
		}, function () {
		$(this).removeClass("active");
	});
	$(".trigger").click(function(){
		$(this).next(".toggle-container").slideToggle();
	});

	$(".trigger.opened").toggle(function(){
		$(this).removeClass("active");
		}, function () {
		$(this).addClass("active");
	});

	$(".trigger.opened").addClass("active").next(".toggle-container").show();


/*----------------------------------------------------*/
/*	Tooltip
/*----------------------------------------------------*/

	$(".tooltip").tooltip({
		position: {
			my: "center bottom-5",
			at: "center top",
			using: function( position, feedback ) {
				$( this ).css( position );
				$( "<div>" )
					.addClass( "arrow" )
					.addClass( feedback.vertical )
					.addClass( feedback.horizontal )
					.appendTo( this );
			}
		}
	});


/*----------------------------------------------------*/
/*	Isotope Portfolio Filter
/*----------------------------------------------------*/

	$(window).load(function(){
		$('#portfolio-wrapper').isotope({
			  itemSelector : '.isotope-item',
				layoutMode : 'fitRows'
		});
		$('#filters a.selected').trigger("click");
	});
	$('#filters a').click(function(e){
		e.preventDefault();

		var selector = $(this).attr('data-option-value');
		$('#portfolio-wrapper').isotope({ filter: selector });

		$(this).parents('ul').find('a').removeClass('selected');
		$(this).addClass('selected');
	});


/*----------------------------------------------------*/
/*	Skill Bar Animation
/*----------------------------------------------------*/

		setTimeout(function(){

		$('.skill-bar .skill-bar-content').each(function() {
			var me = $(this);
			var perc = me.attr("data-percentage");

			var current_perc = 0;

			var progress = setInterval(function() {
				if (current_perc>=perc) {
					clearInterval(progress);
				} else {
					current_perc +=1;
					me.css('width', (current_perc)+'%');
				}

				me.text((current_perc)+'%');

			}, 10);

		});

	},10);


/*----------------------------------------------------*/
/*	Fancybox2
/*----------------------------------------------------*/

	$('[rel=fancybox]').fancybox({
		type        : 'image',
		openEffect  : 'elastic',
		closeEffect	: 'elastic',
		nextEffect  : 'elastic',
		prevEffect  : 'elastic',
		helpers : {
			title : {
				type : 'inside'
			},
			overlay : {
				css : {
					'background' : 'rgba(0, 0, 0, 0.85)'
				}
			}
		}
	});

	$('[rel=fancybox-gallery]').fancybox({
		openEffect  : 'elastic',
		closeEffect	: 'elastic',
		nextEffect  : 'elastic',
		prevEffect  : 'elastic',

		helpers : {
			title : {
				type : 'inside'
			},
			buttons	: {},
			overlay : {
				css : {
					'background' : 'rgba(0, 0, 0, 0.85)'
				}
			}
		},

	});

	
/*----------------------------------------------------*/
/*	Layer Slider
/*----------------------------------------------------*/

	$('#layerslider').layerSlider({
		skin : 'fullwidth',
		hoverPrevNext 			: true,
		navStartStop 			: false,
		navButtons				: false,
		autoPlayVideos			: false,
		animateFirstLayer		: false
					
	});

/*----------------------------------------------------*/
/*	Portfolio Filters
/*----------------------------------------------------*/

	function DropDown(el) {
		this.dd = el;
		this.opts = this.dd.find('ul.option-set > li');
		this.placeholder = this.dd.children('span');
		this.val = [];
		this.index = [];
		this.initEvents();
	}

	DropDown.prototype = {
		initEvents : function() {
			var obj = this;

			obj.dd.on('click', function(event){
				$(this).toggleClass('active');
				event.stopPropagation();
			});
		obj.opts.on('click',function(){
				var opt = $(this);
				obj.val = opt.text();
				obj.index = opt.index();
				obj.placeholder.text('' + obj.val);
			});
		}
	}

	$(function() {

		var dd = new DropDown( $('#filters') );

		$(document).click(function() {
			$('.filters-dropdown').removeClass('active');
		});
				
		$(".option-set").click(function() {
			$('.filters-dropdown').toggleClass('active');
		});

	});


/* ------------------ End Document ------------------ */
});

})(this.jQuery);



(function()
{
	$.fn.pixusNotifications = function(options)
	{
		var defaults = {
			speed: 200,
			animation: 'fade',
			hideBoxes: false
		};

		var options = $.extend({}, defaults, options);

		return this.each(function()
		{
			var wrapper = $(this),
				notification = wrapper.find('.notification'),
				content = notification.find('p'),
				title = content.find('strong'),
				closeBtn = $('<a class="close" href="#"><i class="icon-remove"></i></a>');

			$(document.body).find('.notification').each(function(i)
			{
				var i = i+1;
				$(this).attr('id', 'notification_'+i);
			});

			notification.filter('.closeable').append(closeBtn);

			closeButton = notification.find('> .close');

			closeButton.click(function()
			{
				hideIt( $(this).parent() );
				return false;
			});

			function hideIt(object)
			{
				switch(options.animation)
				{
					case 'fade': fadeIt(object);     break;
					case 'slide': slideIt(object);     break;
					case 'box': boxAnimIt(object);     break;
					case 'fadeAndSlide': fadeItSlideIt(object);     break;
					default: fadeItSlideIt(object);
				}
			};

			function fadeIt(object)
			{	object
				.fadeOut(options.speed);
			}
			function slideIt(object)
			{	object
				.slideUp(options.speed);
			}
			function fadeItSlideIt(object)
			{	object
				.fadeTo(options.speed, 0, function() { slideIt(object) } );
			}
			function boxAnimIt(object)
			{	object
				.hide(options.speed);
			}

			if (options.hideBoxes){}

			else if (! options.hideBoxes)
			{
				notification.css({'display': 'block', 'visiblity': 'visible'});
			}

		});
	};
})();


/**
 * jQuery Plugin to obtain touch gestures from iPhone, iPod Touch and iPad, should also work with Android mobile phones (not tested yet!)
 * Common usage: wipe images (left and right to show the previous or next image)
 *
 * @author Andreas Waltl, netCU Internetagentur (http://www.netcu.de)
 * @version 1.1.1 (9th December 2010) - fix bug (older IE's had problems)
 * @version 1.1 (1st September 2010) - support wipe up and wipe down
 * @version 1.0 (15th July 2010)
 */
(function($){$.fn.touchwipe=function(settings){var config={min_move_x:20,min_move_y:20,wipeLeft:function(){},wipeRight:function(){},wipeUp:function(){},wipeDown:function(){},preventDefaultEvents:true};if(settings)$.extend(config,settings);this.each(function(){var startX;var startY;var isMoving=false;function cancelTouch(){this.removeEventListener('touchmove',onTouchMove);startX=null;isMoving=false}function onTouchMove(e){if(config.preventDefaultEvents){e.preventDefault()}if(isMoving){var x=e.touches[0].pageX;var y=e.touches[0].pageY;var dx=startX-x;var dy=startY-y;if(Math.abs(dx)>=config.min_move_x){cancelTouch();if(dx>0){config.wipeLeft()}else{config.wipeRight()}}else if(Math.abs(dy)>=config.min_move_y){cancelTouch();if(dy>0){config.wipeDown()}else{config.wipeUp()}}}}function onTouchStart(e){if(e.touches.length==1){startX=e.touches[0].pageX;startY=e.touches[0].pageY;isMoving=true;this.addEventListener('touchmove',onTouchMove,false)}}if('ontouchstart'in document.documentElement){this.addEventListener('touchstart',onTouchStart,false)}});return this}})(jQuery);
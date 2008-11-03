   (function($) {
   $.fn.page = function(options) {
	   var defaults={
		   	   direction: 'next'
		   };
		   var opts= $.extend($.fn.page.defaults,defaults,options);
		 if( $(this).get(0).disabled!=true){
		   $(opts.subject).hide('slow',function(){opts.func(opts.direction) });
	  		 	   $(opts.subject).show('slow');
  }

  };  
 $.fn.page.defaults = {
func : new Function(),
subject: "#main_inner" };

})(jQuery)
	 
	 
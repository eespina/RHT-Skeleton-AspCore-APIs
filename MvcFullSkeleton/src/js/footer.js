function footerReady(){
	// $(window).resize(function() {
	// 	if(isDesktop()){
	// 		// showAllAccordionGroup('accordContainer');
	// 	}else{
	// 		// hideAllAccordionGroup('accordContainer');	
	// 	}
	// });
}

function toggleFooter(e){
	if(isDesktop()){
		e.preventDefault();
		e.stopImmediatePropagation();
	}
}
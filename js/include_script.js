// include('request_url.js');
//----jquery-plagins----
include('jquery-1.8.3.min.js');
include('jquery.ba-resize.min.js');
//----bootstrap----

//----All-Scripts----
include('jquery.mobilemenu.js');
include('scroll_to_top.js');
include('ajax.js.switch.js');
include('mathUtils.js');
include('jquery.fancybox.pack.js');
include('jquery.tmpl.min.js');
//----------------------------
include('jquery.easing.1.3.js');
include('hoverSprite.js');
include('jquery.animate-colors-min.js');
include('spin.js');
include('forms.js');
include('bgStretch.js');
include('quiniela.js');
include('bootstrap-tooltip.js');
include('bootstrap-collapse.js');

include('_grid.js');
include('_LINQtoJquery.js');

include('script.js');
//----Include-Function----
function include(url){ 
  document.write('<script type="text/javascript" src="/js/'+ url + '"></script>'); 
  return false;
}

var currentExpressCompnanyData = null;
var printObjects = [];
var templateData = null;

$(document).ready(function(){
	var i = 0;
	for(i=0;i<expressCompnany_datas.length;i++){
		$('#expressCompnany').append('<option value="'+expressCompnany_datas[i].id+'">'+expressCompnany_datas[i].name+'</option>');
	}
	$('#expressCompnany').change(function(){
		printObjects = [];
		templateData = null;
		$('.infoCheck').attr('checked',false);
		$('.object').remove();
		$('#content').html('');
		var expressCompnany = $(this).val();
		initContent(expressCompnany);
	});
	for(var key in info){
		var arr = info[key];
		var html = '';
		for(i=0;i<arr.length;i++){
			html+='<label class="checkbox"><input type="checkbox" class="infoCheck" id="'+arr[i].id+'">'+arr[i].name+'</label>';
		}
		$('#'+key).html(html);
	}
	$('.infoCheck').change(function(){
		var a = $(this).is(':checked');
		var id = $(this).attr('id');
		var cb = $(this);
		if(a){
			var data = {};
			var contentTop = $('#content').offset().top;
			var contentLeft = $('#content').offset().left;
			if(currentExpressCompnanyData == null){
				for(i=0;i<expressCompnany_datas.length;i++){
					if(expressCompnany_datas[i].id==$('#expressCompnany').val()){
						currentExpressCompnanyData = expressCompnany_datas[i];
						break;
					}
				}
			}
			if(currentExpressCompnanyData!=null && currentExpressCompnanyData.objects!=null){
				var arr = currentExpressCompnanyData.objects;
				for(var k = 0;k<arr.length;k++){
					if(arr[k].name == id){
						data = arr[k];
					}
				}
			}
			if(data.text==null || data.text==''){
				if(data.name == null && id.indexOf('custom')==0){//新建的自定义属性
					var _index = layer.prompt({title: '请输入自定义的内容'},function(content){
					    data.text = content;
					    layer.close(_index);
					    _buildPrintObject();
					},function(){
						cb.attr('checked',false);
					});
				}else{
					data.text = $(this).parent().text();
					_buildPrintObject();
				}
			}else{
				_buildPrintObject();
			}
			function _buildPrintObject(){
				if(data.w==null){
					data.w = 'auto';
				}
				if(data.h == null){
					data.h = 'auto';
				}
				if(data.top == null){
					//data.top = contentTop+printObjects.length*15;
					data.top = 0;
				}
				if(data.left == null){
					data.left = 0;
				}
				if(data.name == null){
					data.name = id;
				}
				if(data['font-size']==null){
					data['font-size'] = $('#fontSize').val();
				}
				if(data['font-family']==null){
					data['font-family'] = $('#fontStyle').val();
				}
				var object = new PrintObject(data,$('#content'));
				printObjects.push(object);
			}
		}else{
			if(printObjects!=null && printObjects.length>0){
				for(var i=0;i<printObjects.length;i++){
					if(printObjects[i].id==id){
						var tempObj = printObjects[i];
						tempObj.remove();
						printObjects.splice(i,1);
						tempObj = null;
						break;
					}
				}
			}
		}
	})
	//界面的一些设置框
	for(var b=8;b<=25;b++){
		$('.fontSizes').append('<option value="'+b+'px" '+(b==16?'selected':'')+'>'+b+'</option>')
	}
	$('.aboutObject').hide();
	$('.styleSelector').change(function(){
		var val = $(this).val();
		var styleName = $(this).attr('data_styleName');
		for(var i=0;i<printObjects.length;i++){
			if('all'==$(this).attr('data_scope') || printObjects[i].selected){
				var data = {};
				data[styleName] = val;
				printObjects[i].setStyle(data);
			}
		}
	})
	//$('#btnSaveWaybillPrintConfig').click(function () {
	//	if($('#expressCompnany').val()==''){
	//		layer.alert('请选择快递公司', 9);
	//		return;
	//	}
	//	$.ajax({
	//	    url: '/WaybillPringConfig/Maintain',
	//		type:'POST',
	//		data:{templateData:JSON.stringify(buildTemplate())},
	//		dataType:'text',
	//		cache:false,
	//		async:true,
	//		timeout:36000000,
	//		beforeSend: function(){
	//			//$(this).button('loading');
	//	    },
	//	    complete: function(){
	//	    	//$(this).button('reset');
	//	    },
	//	    success: function(data){
	//		    var jsonData = JSON.parse(data);
	//		    var message = jsonData.msg;
	//		    if(jsonData.result=='success'){
	//		    	 message = '保存成功';
	//			}
	//			layer.alert(message, 9);
	//	    }
	//	});
	//})
	//
	if(paramExpress!=null && paramExpress!=''){
		$('#expressCompnany').val(paramExpress);
		//$('#expressCompnany').change();
	}
	$('#imgWidth').change(function(){
		var v = $(this).val();
		if(v!='' && isNaN(v)==false && $('#expressImg').length>0){
			$('#expressImg').css('width',(v*3.78)+'px');
			$('#expressImg').find('img').css('width',(v*3.78)+'px');
		}
		
	})
	$('#imgHeight').change(function(){
		var v = $(this).val();
		if(v!='' && isNaN(v)==false && $('#expressImg').length>0){
			$('#expressImg').css('height',(v*3.78)+'px');
			$('#expressImg').find('img').css('height',(v*3.78)+'px');
		}
		
	})
	$('#content').click(function(e){
		$('#objectSetting').hide();
		
	})
})

function initContent(expressCompnany, printTextObject) {
	if(templateData==null){
		for(i=0;i<expressCompnany_datas.length;i++){
			if(expressCompnany_datas[i].id==expressCompnany){
				currentExpressCompnanyData = expressCompnany_datas[i];
				break;
			}
		}
	}else{
		currentExpressCompnanyData = {};
		var printContent = decodeURIComponent(templateData.printContent);
		
		$('#content').html(printContent);
		var objects = [];
		$('.object').each(function(){
			var item = {name:$(this).attr('id').split('_')[0],top:$(this).css('top'),left:$(this).css('left'),
			'font-size':$(this).css('font-size'),'font-weight':$(this).css('font-weight'),'font-family':$(this).css('font-family')};
			if($(this).text()!=null && $(this).text().indexOf('{{>')==-1){
				item.text = $(this).text();
			}

			if (printTextObject && printTextObject[item.name]) {
			    item.text = printTextObject[item.name];
			}

			if('auto'==$(this).attr('data-w')){
				item.w = 'auto';
				item.h = 'auto';
			}else{
				item.w = $(this).css('width');
				item.h = $(this).css('height');
			}
			objects.push(item);
		})
		$('.object').remove();
		currentExpressCompnanyData.objects = objects;
	}
	if(currentExpressCompnanyData!=null){
	    $('#content').append('<div id="expressImg" class="noprint"><img  class="noprint" src="/content/scripts/WaybillPrintConfigMgt/printer/images/' + expressCompnany + '.jpg"/></div>');
		var objects = currentExpressCompnanyData.objects;
		if(objects!=null && objects.length>0){
			for(var k=0;k<objects.length;k++){
				$('#'+objects[k].name).click();
			}
		}
		if($('#expressImg').find('img').length>0){
			$('#expressImg').find('img').load(function(){
				if(templateData!=null && templateData.imageWidth!=null && templateData.imageWidth!='' && isNaN(templateData.imageWidth)==false){
					$('#imgWidth').val(templateData.imageWidth);
					$('#expressImg').css('width',(templateData.imageWidth*3.78)+'px');
					$('#expressImg').find('img').css('width',(templateData.imageWidth*3.78)+'px');
				}else{
					$('#imgWidth').val(Math.round($('#expressImg').find('img').width()/3.78));
				}
				if(templateData!=null && templateData.imgHeight!=null && templateData.imgHeight!='' && isNaN(templateData.imgHeight)==false){
					$('#imgHeight').val(templateData.imgHeight);
					$('#expressImg').css('height',(templateData.imgHeight*3.78)+'px');
					$('#expressImg').find('img').css('height',(templateData.imgHeight*3.78)+'px');
				}else{
					$('#imgHeight').val(Math.round($('#expressImg').find('img').height()/3.78));
				}
				currentExpressCompnanyData = null;
				templateData = null;
			})
		}else{
			currentExpressCompnanyData = null;
			templateData = null;
		}
	}else{
		currentExpressCompnanyData = null;
		templateData = null;
	}
	
}

function clearPrintObjectSelected(){
	for(var i=0;i<printObjects.length;i++){
		if(printObjects[i].selected){
			printObjects[i].unselected();
		}
	}
}

function buildTemplate(){
	var result = {};
	result.printMac = $('#printMac').val();
	result.expressCompnany = $('#expressCompnany').val();
	result.imageWidth = $('#imgWidth').val();
	result.imgHeight = $('#imgHeight').val();
	result.printWidth = $('#printWidth').val();
	result.printHeight = $('#printHeight').val();
	var printContent = '';
	for(var i=0;i<printObjects.length;i++){
		printObjects[i].unselected();
		printContent+= printObjects[i].html(true);
	}
	result.printContent = encodeURIComponent(printContent);
	return result;
}

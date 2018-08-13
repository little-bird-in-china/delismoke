var PrintObject = function(data,container){
	//类方法
	if(typeof PrintObject._initialized == "undefined"){
    	PrintObject.prototype.doselected = function(){
    		$('.aboutObject').hide();
     		if(this.selected &&  this.moveed == false){
     			this.unselected();
     		}else{
     			clearPrintObjectSelected();
     			this.el.addClass('selected');
     			this.selected = true;
     			$('.aboutObject').show();
     			var thisObj = this;
     			$('.styleSelector').each(function(){
     				var styleName = $(this).attr('data_styleName');
     				if(thisObj[styleName]!=null){
     					$(this).val(thisObj[styleName]);
     				}
     			})
     		}
    	};
    	PrintObject.prototype.unselected = function(){
    		this.el.removeClass('selected');
     		this.selected = false;
    	};
    	PrintObject.prototype.setStyle = function(styleData){
    		for(var key in styleData){
    			this[key] = styleData[key];
    			this.el.css(key,styleData[key]);
    		}
    	};
    	PrintObject.prototype.html = function(useIdValue){
    		var html = '<div id="'+this.id+'_obj" class="object" '+(true!=this.resized?'data-w="auto"':'')+
    				   'style="'+(true!=this.resized?'width:auto;height:auto':'width:'+this.w+';height:'+this.h)+';top:'+this.top+';left:'+this.left+';position:absolute;'+
    				   'font-size:'+this['font-size']+';font-weight:'+this['font-weight']+';font-family:'+this['font-family']+';text-align:left;">'+
    				   '<i class="delBut icon-remove" style="float:left;"></i>'+((useIdValue && this.id.indexOf('custom')==-1)?'{{>'+this.id.replace('Check','')+'}}':this.text)+'</div>';
    		//alert(html)
    		return html;
    	};
    	PrintObject.prototype.remove = function(){
    		this.el.remove();
    	};
    	PrintObject.prototype.toPx = function(val){
    		val = val+"";
    		if(val!=null && val.indexOf('px')==-1 && val!='auto'){
    			val = val+'px';
    		}
    		return val;
    	}
    }
    PrintObject._initialized = true;
    //类方法定义结束
	var obj = this;
	this.id = data.name;
	this.text = data.text;
	this.w = this.toPx(data.w);
	this.h = this.toPx(data.h);
	if(this.w!='auto'){
		this.resized = true;
	}
	this.top = this.toPx(data.top);
	this.left = this.toPx(data.left);
 
	this['font-size'] = data['font-size']!=null?data['font-size']:'12px';
	this['font-weight'] = data['font-weight']!=null?data['font-weight']:'normal';
	this['font-family'] = data['font-family']!=null?data['font-family']:'新宋体';
	if(container!=null){
		this.parent = container;
		this.el = $(this.html());
		this.parent.append(this.el);
		this.el.draggable({drag:function(){
			$('#objectSetting').hide();
		}}).resizable({minHeight: 10,minWidth: 50,stop:function(){
			obj.resized = true;
		}});
		this.el.find('.delBut').click(function(){
			$('#'+obj.id).click();
		})
		this.el.mousedown(function(){
			obj.mousedowned = true;
		})
		this.el.mouseup(function(){
			//$(this).text(($(this).offset().top-$('#content').offset().top)+','+($(this).offset().left-$('#content').offset().left));
			obj.doselected();
			obj.mousedowned = false;
			obj.moveed = false;
			obj.top = ($(obj.el).offset().top-$(obj.parent).offset().top+$(obj.parent).scrollTop())+'px';
			obj.left = ($(obj.el).offset().left-$(obj.parent).offset().left+$(obj.parent).scrollLeft())+'px';
			obj.w = $(obj.el).width()+'px';
			obj.h = $(obj.el).height()+'px';
			setTimeout(function(){
				$('#objectSetting').show();
				$('#objectSetting').css('top',$(obj.el).offset().top-30);
				$('#objectSetting').css('left',$(obj.el).offset().left);
			},200)
			
		})
		this.el.mousemove(function(){
			if(obj.mousedowned!=null && obj.mousedowned)obj.moveed = true;
		})
		 
	}
	
    return obj;
}


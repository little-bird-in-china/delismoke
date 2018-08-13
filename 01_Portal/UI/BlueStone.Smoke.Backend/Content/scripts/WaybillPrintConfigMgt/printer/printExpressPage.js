var Order = Backbone.Model.extend({
	url:'/printer/printerAction!tradeDeal.action',
	initialize : function(){
		
	}
});
var OrdersList =  Backbone.Collection.extend({
	model : Order,
	url:'/printer/printerAction!printExpressPage.action?isDefault=no'
});
var Orders = new OrdersList();
var OrdersRowView = MyView.extend({
	tagName : 'tr',
	template:$("#ordersRow-template" ),
	initialize : function(){
		this.model.bind('change',this.render,this);//model发生改变,便重新渲染
	},
	events :{
		"click .printPriview" : "printPriviewHandler",
		"click .print" : "printHandler",
		"click .viewOrderDest" : "viewOrderDestHandler",
		"click .cancelMerge" : "cancelMergeHandler",
		"click .isShowOrderDest" : "isShowOrderDestHandler",
		"click .editReceiverInfo" : "editReceiverInfoHandler"
	},
	render: function() {
		var html=this.template.render(this.model.toJSON());
	    $(this.el).html(html);
	    $('a[rel="popover"]').popover();
	    return this;
    },
    printPriviewHandler:function(){
    	this.doPrint(true);
    },
    printHandler : function(){
    	var thisObj = this;
    	if((this.model.get('printTime')!=null && this.model.get('printTime')!='') || 
    	 (domainId=='taobao' && 'WAIT_SELLER_SEND_GOODS'!=this.model.get('tradeStatus')) ||
    	 (domainId=='paipai' && 'DS_WAIT_SELLER_DELIVERY'!=this.model.get('tradeStatus'))){
    		var win = layer.confirm('订单已发货或打印，是否重复打印？',function(){
    			thisObj.doPrint();
    			layer.close(win);
    		})
    	}else{
    		this.doPrint();
    	}
    	
    },
    doPrint:function(isPriview){
    	//alert(MYLODOP)
    	if(appV.printAble()==false)return;
    	var tid = this.model.get('tid');
    	var content = '';
    	var expressCompnany = baseInfo.deaultExpress;
    	//baseInfo.pageWidth*10,baseInfo.pageHeight*10
    	if(isPriview){
    		content += '<img src="/js/system/printer/images/'+expressCompnany+'.jpg" style="position:absolute;top:0px;left:0px;';
    		if(baseInfo.pageWidth!=null && baseInfo.pageWidth!=''){
    			content +='width:'+Math.round(baseInfo.pageWidth*3.78)+'px;';
    		}
    		if(baseInfo.pageHeight!=null && baseInfo.pageHeight!=''){
    			content +='height:'+Math.round(baseInfo.pageHeight*3.78)+'px;';
    		}
    		content+='"/>';
    	}
    	var thisObj = this;
    	this.model.fetch({param:{type:'getPrintContent',tid:tid,expressCompnany:expressCompnany},success:function(model,data){
		    	var result = eval('('+data+')');
		    	if(result.result=='success'){
		    		if(result.content==null || result.content==''){
		    			//用户无定义
		    			for(i=0;i<expressCompnany_datas.length;i++){
							if(expressCompnany_datas[i].id==expressCompnany){
								var currentExpressCompnanyData = expressCompnany_datas[i];
								var objects = currentExpressCompnanyData.objects;
								if(objects!=null && objects.length>0){
									for(var k=0;k<objects.length;k++){
										content+=new PrintObject(objects[k],null).html(true);
									}
								}
								break;
							}
						}
		    		}else{
		    			content += decodeURIComponent(result.content);
		    		}
		    		
		    		var temlateData = {};
		    		if(result.tradeDatas!=null &&  result.tradeDatas.length>0){
		    			temlateData = result.tradeDatas[0];
		    		}
		    		if(baseInfo!=null && baseInfo!=''){
		    			for(var key in baseInfo){
		    				temlateData[key] = baseInfo[key];
		    			}
		    		}
		    		//alert(html)
	    			MYLODOP.PRINT_INIT("打印快递单");
	    			content = appV.initLODOP(content);
		    		var html = $.templates(content).render(temlateData);
					MYLODOP.ADD_PRINT_HTM(0,0,'100%','100%',html);
		    		if(isPriview){
		    			//MYLODOP.ADD_PRINT_SETUP_BKIMG("<img border='0' src='/js/system/printer/images/"+expressCompnany+".jpg'>");
		    			//MYLODOP.SET_SHOW_MODE("BKIMG_IN_PREVIEW",1);
		    			MYLODOP.SET_PREVIEW_WINDOW(0,3,0,1200,600,"");
						MYLODOP.SET_SHOW_MODE("PREVIEW_NO_MINIMIZE",true);//预览窗口禁止最小化，并始终最前
		    			MYLODOP.PREVIEW();
		    		}else{
		    			if(isIE || is64IE || confirm('数据加载完毕，开始打印?')){
		    				MYLODOP.SET_PRINT_MODE("CATCH_PRINT_STATUS",true);
		    				P_ID = MYLODOP.PRINT();
		    				if (P_ID!="" && MYLODOP.GET_VALUE("PRINT_STATUS_EXIST",P_ID)==1) {
		    					appV.printCompleteHandler(tid);
		    				}
		    			}
	    				
		    		}
		    	}else{
		    		layer.alert(result.msg);
		    	}
	    	}})
    },
    cancelMergeHandler : function(e){
    	this.model.fetch({param:{type:'cancelMerge',tid:this.model.get('tid')},beforeSend:function(){$(e.currentTarget).button('loading')},
    		success:function(model,data){
		    	var result = eval('('+data+')');
		    	if(result.result!='error'){
		    		appV.refleshOrders();
		    		layer.alert('取消合并成功');
		    	}else{
		    		layer.alert(result.msg);
		    	}
	    	}})
    },
    isShowOrderDestHandler : function(e){
    	var input = $(e.currentTarget);
    	var thisObj = this;
    	if(this.model.get('trades')!=null){
    		thisObj.model.set({isShowOrderDest:input.is(':checked')?'1':'0'});
    	}else{
    		this.model.fetch({param:{type:'getTradeDest',tid:this.model.get('tid')},beforeSend:function(){input.button('loading')},success:function(model,data){
		    	var result = eval('('+data+')');
		    	if(result.result!='error'){
		    		if(result.orderDest!=null && result.orderDest!=''){
		    			var arr = result.orderDest;
		    			for(var i=0;i<arr.length;i++){
		    				arr[i].orders = eval(decodeURIComponent(arr[i].orders));
		    				arr[i].ordersLen = arr[i].orders.length;
		    				//alert(JSON.stringify(arr[i]))
		    			}
		    		}
					thisObj.model.set({isShowOrderDest:input.is(':checked')?'1':'0',trades:result.orderDest});
		    	}else{
		    		layer.alert(result.msg);
		    	}
	    	}})
    	}
    },
    editReceiverInfoHandler : function(){
    	var thisObj = this;
    	var _index = $.layer({
		    type: 1,
		    maxmin: true,
		    shadeClose: false,
		    title: '编辑订单'+this.model.get('tid')+'收件人信息',
		    shade: [0.1,'#fff'],
		    offset: ['20px',''],
		    area: ['640px', '400px'],
		    page: {html:$('#editReceiverInfo-template').render(this.model.toJSON())}
		});
		$('#saveReceiverInfoBut').click(function(){
			var param = {type:'saveReceiverInfo',tid:thisObj.model.get('tid')};
			param.receiverName = $('#edit_receiverName').val();
			param.receiverMobile = $('#edit_receiverMobile').val();
			param.receiverZip = $('#edit_receiverZip').val();
			param.receiverState = $('#edit_receiverState').val();
			param.receiverCity = $('#edit_receiverCity').val();
			param.receiverDistrict = $('#edit_receiverDistrict').val();
			param.receiverAddress = $('#edit_receiverAddress').val();
			param.sellerMemo = $('#edit_sellerMemo').val();
			
			thisObj.model.fetch({param:param,beforeSend:function(){$(this).button('loading')},success:function(model,data){
		    	var result = eval('('+data+')');
		    	if(result.result!='error'){
					thisObj.model.set(param);
					layer.close(_index);
		    	}else{
		    		layer.alert(result.msg);
		    	}
	    	}})
		});
		$('#closeReceiverInfoWin').click(function(){
			layer.close(_index);
		})
    }
});

var OrdersListView = MyView.extend({
	el : $('#listDiv'),
	template:$("#ordersList-template"),
	config:{},
	initialize : function(){
		this.model.bind('update',this.render,this);
		this.config = this.options.config;
	},
	events : {
		"click #setToPrinted" : "setToPrintedHander",
		"click #mergeOrders" : "mergeOrdersHandler",
		"click #printSetBut" : "printSetHandler",
		"click #chooseAll" : "chooseAllHandler"
	},
	render: function() {
		
		var html=this.template.render(this.config);
	    $(this.el).html(html);
	    var arr = this.model.models;
	    var j=1;
	    for(var i=0;i<arr.length;i++){
	    	if(arr[i]!=null && arr[i].get('tid')!=null){
	    		arr[i].set({index:j});
	    		this.showOrdersRow(arr[i]);
	    		j++;
	    	}
	    }
	    var thisObj = this;
	    if(true!=this.config.loadingOrders){
	    	dpUtil.displayPageBar($('#pageBar'),this.config['totalRows'],this.config['totalPage'],this.config['currentPage'],
				function(type,page){
					if(page!=thisObj.config.currentPage){
			    		thisObj.config.loadingOrders=true;
			    		thisObj.config.currentPage=page;
				    	thisObj.render();
				    	appV.showOrdersList(thisObj.config);
			    	}
					
				});
	    }
	    $('.funcTip').tooltip();
	    return this;
    },
    showOrdersRow : function(order){
    	var table = $(this.el).find('#mainTable');
		var view = new OrdersRowView({model:order});
		view.render();
		//table.append($(view.el).html());
		table.append(view.el)
		return view;
	},
	hide : function(){
		$(this.el).css('display','none');
	},
	show : function(){
		$(this.el).css('display','block');
	},
	isSaveReceiver : function(){
		var receiverName,receiverAddress;
		var result = true;
		$('.rowCb:checked').each(function(){
			if($(this).is(':checked')){
				var tempReceiverName = $(this).attr('data-receiverName');
				var tempReceiverAddress = $(this).attr('data-receiverAddress');
				if(receiverName==null)receiverName=tempReceiverName;
				if(receiverAddress==null)receiverAddress=tempReceiverAddress;
				if(receiverName!=tempReceiverName || receiverAddress!=tempReceiverAddress){
					result = false;
					return;
				}
			}
		});
		return result;
	},
	setToPrintedHander : function(e){
		var tid = appV.findCheckedTids();
		if(tid==''){
			layer.alert('请选择要设置为已打印的订单');
			return;
		}
		
		Orders.fetch({param:{type:'setToPrinted',tid:tid},beforeSend:function(){$(e.currentTarget).button('loading')},
			success:function(model,data){
		    	var result = eval('('+data+')');
		    	$(e.currentTarget).button('reset')
		    	if(result.result=='success'){
		    		appV.refleshOrders();
		    		layer.alert('设置为已打印成功');
		    	}else{
		    		layer.alert(result.msg);
		    	}
	    	}});
	},
	mergeOrdersHandler : function(e){
		//判断是否是同一个收货人,同一个收货地址
		var tid = appV.findCheckedTids();
		if(tid==''){
			layer.alert('请选择要合并的订单');
			return;
		}
		if($('.rowCb:checked').length<2){
			layer.alert('至少选择两个订单才能合并');
			return;
		}
		if(this.isSaveReceiver()==false){
			layer.alert('所选择订单的收货人或收贷地址不全相同，无法合并');
			return;
		}
		var but = $(e.currentTarget);
		Orders.fetch({param:{type:'mergeOrders',tid:tid},beforeSend:function(){$(e.currentTarget).button('loading')},
			success:function(model,data){
		    	var result = eval('('+data+')');
		    	$(e.currentTarget).button('reset')
		    	if(result.result=='success'){
		    		appV.refleshOrders();
		    		layer.alert('合并成功');
		    	}else{
		    		layer.alert(result.msg);
		    	}
	    	}});
	},
	printSetHandler : function(e){
		var settingWin = $.layer({
		    type: 1,
		    maxmin: true,
		    shadeClose: false,
		    title: '打印设置',
		    shade: [0.1,'#fff'],
		    offset: ['20px',''],
		    area: ['640px', '400px'],
		    page: {html:'<div id="printSetting"></div>'}
		});
		buildPrintSetting();
		function buildPrintSetting(){
			if($('#paddingLeft')[0]!=null){
				baseInfo.pageWidth = $('#pageWidth').val();
				baseInfo.pageHeight = $('#pageHeight').val();
				baseInfo.paddingLeft = $('#paddingLeft').val();
				if(isNaN(baseInfo.paddingLeft))baseInfo.paddingLeft=0;
				baseInfo.paddingTop = $('#paddingTop').val();
				if(isNaN(baseInfo.paddingTop))baseInfo.paddingTop=0;
				baseInfo.deaultExpress = $('#deaultExpress').val();
				baseInfo.deaultExpressName = $('#deaultExpress option:selected').text();
				baseInfo.usePageType = $('#pageType').val();
				baseInfo.usePrintMac = $('#printMac').val();
				
			}
			$('#printSetting').html($('#printSetting-template').render(baseInfo));
			$('#printMac').change(function(){
				baseInfo.usePrintMac = $(this).val();
				baseInfo.pageTypes = [];
				if($(this).val()!='-1'){
					var Options=getPageTypes($(this).val()); 
				    for (i in Options)    
				    {    
			   		  baseInfo.pageTypes.push({id:Options[i],name:Options[i]});
				    }  
				}
				buildPrintSetting();
			})
			$('#pageType').change(function(){
				baseInfo.usePageType = $(this).val();
				buildPrintSetting();
			})
			$('#savePrintsetting').click(function(){
				buildPrintSetting();
				var settingInfo = {};
				for(var key in baseInfo){
					if(key!='printMacs' && key!='expressCompnanyDatas' && key!='pageTypes'){
						settingInfo[key] = baseInfo[key];
					}
				}
				Orders.fetch({param:{type:'savePrintsetting',settingInfo:JSON.stringify(settingInfo)},beforeSend:function(){$(e.currentTarget).button('loading')},
					success:function(model,data){
				    	var result = eval('('+data+')');
				    	$(e.currentTarget).button('reset')
				    	if(result.result=='success'){
				    		appV.refleshOrders();
				    		layer.alert('保存成功');
				    		layer.close(settingWin);
				    	}else{
				    		layer.alert(result.msg);
				    	}
			    	}});
			})
			$('#closePrintSetting').click(function(){
				layer.close(settingWin)
			})
			$('#gotoExpressDesigner').click(function(){
				$(this).attr('href','/printer/printerAction!printDesigner.action?express='+$('#deaultExpress').val());
			})
			$('#deaultExpress').change(function(){
				Orders.fetch({param:{type:'getSimpleTempate',expressCompnany:$(this).val()},
					success:function(model,data){
				    	var result = eval('('+data+')');
				    	if(result.result=='success'){
				    		if(result.templateData!=null && result.templateData!=''){
				    			$('#pageWidth').val(result.templateData.imageWidth);
								$('#pageHeight').val(result.templateData.imgHeight);
				    		}else{
				    			
				    		}
				    	}
			    	}});
			})
		}
	},
	chooseAllHandler : function(e){
		$('.rowCb').each(function(){
			$(this)[0].checked = $(e.currentTarget).is(':checked');
		})
	}
});

var AppView = MyView.extend({
	el : $("body"),
	events : {
		"click #syncOrdersBut":"syncOrdersHandler",
		"click #setToDefalutExpress":"setToDefalutExpressHandler",
		"click #searchBut" : "search",
		"click #batchPrint" : "batchPrintHandler",
		"click #saveTraceNumberBut" : "saveTraceNumberHandler",
		"click #fastToInputTrackNumber" : "fastToInputTrackNumberHandler",
		"click #resetBut" : "resetHandler"
	},
	initialize: function() {
		
	},
	syncOrdersHandler : function(e){
		if(needAuth && 'taobao'==parent.domainId){
			try{
				if(!TaobaoShortAuth(parent.appKey,parent.ServerName,parent.nick)){
					return false;
				}
			}catch(err){alert(err)}
		}
		Orders.fetch({param:{type:'syncOrders',isAutoMergeOrders:($('#isAutoMergeOrders').is(':checked')?"1":"0"),lastSyncOrdersTime:lastSyncOrdersTime},beforeSend:function(){$(e.currentTarget).button('loading')},
			success:function(model,data){
		    	var result = eval('('+data+')');
		    	$(e.currentTarget).button('reset')
		    	if(result.result=='success'){
		    		$('#syncTip').html($('#syncTip-template').render(result.baseInfo));
		    		for(var key in result.baseInfo){
		    			baseInfo[key] = result.baseInfo[key];
		    		}
		    		lastSyncOrdersTime = result.baseInfo.lastSyncOrdersTime;
		    		appV.refleshOrders();
		    	}else{
		    		layer.alert(result.msg);
		    	}
	    	}});
	},
	doSyncOrder : function(e){
		
	},
	setToDefalutExpressHandler : function(e){
		if($(e.currentTarget).is(':checked')){
			var expressCompnany = baseInfo.deaultExpress;
	    	if(expressCompnany==null || expressCompnany==''){
	    		showToolTip($('#expressCompnany'),'请选择快递模板');
	    		$(e.currentTarget).attr('checked',false);
	    		return;
	    	} 
			var win = layer.confirm('确定要将'+$('#expressCompnany option:selected').text()+'设为默认快递吗？',function(){
				layer.close(win);
				Orders.fetch({param:{type:'setToDefalutExpress',deaultExpress:expressCompnany},beforeSend:function(){
						parent.displayLoadingBar(null,null,'正在设置...');
					},
					success:function(model,data){
				    	var result = eval('('+data+')');
				    	parent.displayLoadingBar(null,null,'hide');
				    	if(result.result=='success'){
				    		 
				    	}else{
				    		layer.alert(result.msg);
				    	}
			    	}});
			},function(){
				layer.close(win);
				$(e.currentTarget).attr('checked',false);
			})
		}
		
	},
	listV : null,
	clearViewEvent : function(){
		if(this.listV!=null){
			this.listV.undelegateEvents();
		}
	},
	showOrdersList : function(config){
		var param ={type:'search'};
		if(config!=null){
			for(var key in config){
				param[key] = config[key];
			}
		}else{
			config={};
		}
		if(appV.listV!=null){
			appV.clearViewEvent();
		}
		Orders.fetch({param:param,complete:function(){config.loadingOrders=false;},success:function(model,data){
			 	Orders.reset();
		    	var result = eval('('+data+')');
		    	if(result.result=='success'){
		    		var list = result.lists;
		    		for(var i=0;i<list.length;i++){
		    			var log = new Order();
		    			log.set(list[i]);
		    			Orders.add(log);
		    		}
		    		config.totalPage=result.totalPage;
		    		config.totalRows=result.totalRows;
		    		config.currentPage=result.currentPage;
		    		
		    		config.loadingOrders=false;
		    		appV.listV = new OrdersListView({model:Orders,config:config});
					appV.listV.render();
					dpUtil.buildDatetimepicker();
		    	}else{
		    		$('#searchBut').button('reset');
		    		layer.alert(result.msg);
		    	}
		    }
	    });
	},
	search : function(e){
		var input = $(e.currentTarget);
		input.button('loading');
		appV.listV.config.tid=$('#tid').val();
		appV.listV.config.buyerNick=$('#buyerNick').val();
		appV.listV.config.receiverName=$('#receiverName').val();
		appV.listV.config.tradeStatus=$('#tradeStatus').val();
		appV.listV.config.fromDate=$('#fromDate').val();
		appV.listV.config.toDate=$('#toDate').val();
		appV.listV.config.isSellerMemo=$('#isSellerMemo').val();
		appV.listV.config.sellerMemo=$('#sellerMemo').val();
		appV.listV.config.isBuyerMessage=$('#isBuyerMessage').val();
		appV.listV.config.buyerMessage=$('#buyerMessage').val();
		appV.listV.config.sellerFlag=$('input[name="sellerFlag"]:checked').val();
		appV.listV.config.itemTitle=$('#itemTitle').val();
		appV.listV.config.typeAndQuantity=$('input[name="typeAndQuantity"]:checked').val();
		appV.listV.config.sku=$('#sku').val();
		appV.listV.config.deal_type=$('#deal_type').val();
		appV.listV.config.pageSize = 50;
		
		appV.refleshOrders();
	},
	batchPrintHandler : function(e){
		var tid = appV.findCheckedTids();
		if(tid==''){
			layer.alert('请选择要打印的订单');
			return;
		}
		if(appV.printAble()==false)return;
		var expressCompnany = baseInfo.deaultExpress;
		//每组打印30条
	    var tidArr = tid.split(',');
	    var tempTids = '';
	    var tidGroups  =[];
		var index = 0;
		var defalutContent = null;
	    for(var a=0;a<tidArr.length;a++){
	    	tempTids += ','+tidArr[a];
	    	if((a>0 && (a+1)%30==0) || a==tidArr.length-1){
	    		tidGroups.push(tempTids);
	    		tempTids = '';
	    	}
	    }
		//是否打印过或已发货
		var tempModelArr = Orders.models;
		var hadPrintedOrSended = 0;
		for(var i=0;i<tempModelArr.length;i++){
			if((','+tid+',').indexOf(','+tempModelArr[i].get('tid')+',')!=-1){
				if(tempModelArr[i].get('printTime')!=null || 'WAIT_SELLER_SEND_GOODS'!=tempModelArr[i].get('tradeStatus')){
					hadPrintedOrSended++;
				}
			}
		}
		if(hadPrintedOrSended>0){
			var win = layer.confirm('有'+hadPrintedOrSended+'个订单已打印或发货，是否重复打印？',function(){
				layer.close(win);
				 _doPrint();
			})
		}else{
			 _doPrint();
		}
		//
	   
	    function _doPrint(){
	    	var tids = tidGroups[index];
	    	if(tids==null || tids==''){
	    		return;
	    	}
	    	Orders.fetch({param:{type:'getPrintContents',tid:tids,expressCompnany:baseInfo.deaultExpress},beforeSend:function(){$(e.currentTarget).button('loading')},
		   		success:function(model,data){
			    	var result = eval('('+data+')');
			    	if(result.result!='error'){
			    		var content = '';
			    		if(result.content==null || result.content==''){
			    			//用户无定义
			    			if(defalutContent == null){
			    				for(var i=0;i<expressCompnany_datas.length;i++){
									if(expressCompnany_datas[i].id==expressCompnany){
										var currentExpressCompnanyData = expressCompnany_datas[i];
										var objects = currentExpressCompnanyData.objects;
										if(objects!=null && objects.length>0){
											for(var k=0;k<objects.length;k++){
												content+=new PrintObject(objects[k],null).html(true);
											}
										}
										break;
									}
								}
								defalutContent = content;
			    			}else{
			    				content = defalutContent;
			    			}
			    		}else{
			    			content = decodeURIComponent(result.content);
			    		}
			    		var temlateDataArr = result.tradeDatas;
			    		MYLODOP.PRINT_INIT("打印快递单");
			    		content = appV.initLODOP(content);
			    		var P_ID;
			    		for(var b=0;b<temlateDataArr.length;b++){
			    			var temlateData = temlateDataArr[b];
			    			if(temlateData!=null){
			    				if(baseInfo!=null && baseInfo!=''){
					    			for(var key in baseInfo){
					    				temlateData[key] = baseInfo[key];
					    			}
					    		}
					    		var html = $.templates(content).render(temlateData);
					    		MYLODOP.NewPage();
								MYLODOP.ADD_PRINT_HTM(10,55,'100%','100%',html);
								
								if((b>0 && b%10==0) || b==temlateDataArr.length-1){
									MYLODOP.SET_PRINT_MODE("CUSTOM_TASK_NAME","快递单");//为每个打印单独设置任务名
									MYLODOP.SET_PRINT_MODE("CATCH_PRINT_STATUS",true);
									if(isIE || is64IE || b>10 || confirm('数据加载完毕，开始打印?')){
										P_ID = MYLODOP.PRINT();
									}
						    		
									if(b==temlateDataArr.length-1){
										if (P_ID!=null && P_ID!="" && MYLODOP.GET_VALUE("PRINT_STATUS_EXIST",P_ID)==1) {
					    					appV.printCompleteHandler(tid);
					    				}
									}
								}
			    			}
			    		}
			    		
			    		index = index+1;
			    		if(index<tidGroups.length){
			    			_doPrint();
			    		}else{
			    			$(e.currentTarget).button('reset');
			    		}
			    	}else{
			    		layer.alert(result.msg);
			    		$(e.currentTarget).button('reset');
			    	}
		    	}})
	    }
	},
	printAble : function(){
		if(MYLODOP == null || MYLODOP.VERSION==null){
    		$("html,body").stop().animate({scrollTop:0}, 500 ,function(){
				showToolTip($('#lodopTip'),'请先安装打印控件');
    		});
    		return false;
    	}
    	var expressCompnany = baseInfo.deaultExpress;
    	if(expressCompnany==null || expressCompnany==''){
    		$("html,body").stop().animate({scrollTop:100}, 500 ,function(){
    			showToolTip($('#printSetBut'),'请选择快递模板');
    		});
    		
    		return false;
    	}
    	if(baseInfo.senderName==null || baseInfo.senderName==''){
    		var win = layer.confirm('请先完善寄件人信息',function(){
    			layer.close(win);
    			appV.baseInfoWin = $.layer({
				    type: 2,
				    maxmin: true,
				    shadeClose: false,
				    title: '',
				    shade: [0.1,'#fff'],
				    offset: ['20px',''],
				    area: ['840px', '400px'],
				    iframe: {src: "/printer/printerAction!baseInfo.action"}
				}); 
    		});
    		return false;
    	}
    	return true;
	},
	findCheckedTids : function(){
		var result = '';
		$('.rowCb').each(function(){
			if($(this).is(':checked')){
				if(result!='')result+=',';
				result += $(this).val();
			}
		})
		return result;
	},
	printCompleteHandler : function(tids){
		var tidArr = tids.split(',');
		var printedArr = [];
		for(var i=0;i<tidArr.length;i++){
			if(tidArr[i]!=null && tidArr[i]!=''){
				printedArr.push({tid:tidArr[i]});
			}
		}
		
		Orders.fetch({param:{type:'getLastTrackNumber',expressCompnany:baseInfo.deaultExpress},
			success:function(model,data){
				try{
					var result = eval('('+data+')');
			    	if(result.result!='error' && result.lastTrackNumber!=null && result.lastTrackNumber!=''){
			    		 appV.lastTrackNumber = Number(result.lastTrackNumber);
			    	}
				}catch(err){
				
				}
		    	_showWin();
	    	}});
	    function _showWin(){
	    	appV.saveTrackNumberWin = $.layer({
			    type: 1,
			    maxmin: true,
			    shadeClose: false,
			    title: '打印任务已提交,请记录运单号',
			    shade: [0.1,'#fff'],
			    offset: ['20px',''],
			    area: ['640px', '400px'],
			    page: {html:$('#printComplete-template').render({printedArr:printedArr,lastTrackNumber:appV.lastTrackNumber})}
			});
			appV.printedArr = printedArr;
	    }
		
	},
	saveTraceNumberHandler : function(e){
		var traceNumbers = [];
		var tidArr = appV.printedArr;
		var sendNow = $('#sendNow').is(':checked')?'1':'0';
		for(var i=0;i<tidArr.length;i++){
			var tid = tidArr[i].tid;
			var number = $('#'+tid+'_trackNumber').val();
			if(number!=''){
				//if(isNaN(number)){
				//	showToolTip($('#'+tid+'_trackNumber'),'运单号必须是数字');
				//	return;
				//}
				var jobj = {tid:tid,trackNumber:number,expressCompnanyForPrint:baseInfo.deaultExpressName};
				if('1'==sendNow){
					jobj.expressCompnanyForSend=PrinterUtil.getRealExpressCompnany(baseInfo.deaultExpressName);
				}
				traceNumbers.push(jobj);
			}
		}
		if(traceNumbers.length==0){
			layer.alert('没有要保存的运单号');
			return;
		}
		
		var param = {type:'saveTraceNumber',sendNow:sendNow,traceNumbers:JSON.stringify(traceNumbers)};
		
		Orders.fetch({param:param,beforeSend:function(){$(e.currentTarget).button('loading')},
			success:function(model,data){
		    	var result = eval('('+data+')');
		    	if(result.result=='success'){
		    		layer.close(appV.saveTrackNumberWin);
		    		if(result.errorTids!=null && result.errorTids!=''){
		    			location.href = '/printer/printerAction!batchSend.action?simplePage=1&tids='+result.errorTids;
		    		}else{
		    			appV.refleshOrders();
		    		}
		    	}else{
		    		layer.alert(result.msg);
		    	}
	    	}});
	},
	fastToInputTrackNumberHandler : function(e){
		var tidArr = appV.printedArr;
		var firstNumber = 0;
		for(var i=0;i<tidArr.length;i++){
			var tid = tidArr[i].tid;
			var number = $('#'+tid+'_trackNumber').val();
			if(i==0){
				if(number==''){
					showToolTip($('#'+tid+'_trackNumber'),'起始单号不能为空');
					return;
				}
				if(isNaN(number)){
					showToolTip($('#'+tid+'_trackNumber'),'起始单号必须是数字');
					return;
				}
				firstNumber = Number(number);
			}else{
				$('#'+tid+'_trackNumber').val(firstNumber+i);
			}
			
		}
	},
	resetHandler : function(e){
		$('#searchConfig').find('input[type="text"]').val('');
		$('#searchConfig').find('select').val('');
		$('#searchConfig').find('input[type="radio"]').removeAttr('checked');
	},
	initLODOP : function(content){
		if(baseInfo.usePrintMac!=-1){
			MYLODOP.SET_PRINTER_INDEX(baseInfo.usePrintMac);
		}
		if(baseInfo.usePageType=='define'){
			MYLODOP.SET_PRINT_PAGESIZE(0,baseInfo.pageWidth*10,baseInfo.pageHeight*10,"A4");
		}else{
			MYLODOP.SET_PRINT_PAGESIZE(0,0,0,baseInfo.usePageType);
		}
		if(baseInfo.paddingLeft!=0){
			content = appV.offsetContent(content,'left',Math.round(baseInfo.paddingLeft*3.78));
		}
		if(baseInfo.paddingTop!=0){
			content = appV.offsetContent(content,'top',Math.round(baseInfo.paddingTop*3.78));
		}
		return content;
	},
	offsetContent : function(content,flag,offsetValue){
		var a = 0;
		var b = 0;
		var ss = ',';
		for(var i=0;i<10000;i++){
			b=content.indexOf(flag+':',a);
			if(b==-1)break;
			a = content.indexOf(';',b);
			if(a>b && a-b<20){
				var s1 = content.substring(b,a);
				if((ss+',').indexOf(s1)==-1){
					ss+=s1+',';
				}
			}
		}
		ss = ss.split(',');
		for(i=0;i<ss.length;i++){
			if(ss[i]!=null && ss[i]!=''){
				var c = ss[i].indexOf('px');
				if(c==-1)c=ss[i].length;
				var s = ss[i].substring(flag.length+1,c);
				var s1 = Number(s)+Number(offsetValue);
				var s2 = ss[i].replace(s,s1);
				eval("var re = /" + ss[i] + "/g;");
				content = content.replace(re,s2);
			}
		}
		return content;
	},
	baseInfoSetSuccess : function(data){
		layer.close(appV.baseInfoWin);
		if(data!=null){
			for(var key in data){
				baseInfo[key] = data[key];
			}
		}
	},
	refleshOrders : function(){
		appV.listV.config.loadingOrders = true;
		appV.showOrdersList(appV.listV.config);
	}
});
var appV;
var needAuth = true;
parent.displayLoadingBar(null,null,'hide');
$(document).ready(function(){
	$('#syncTip').html($('#syncTip-template').render(baseInfo));
	appV = new AppView();
	appV.showOrdersList({pageSize:50,deal_type:'unprinted',tradeStatus:'WAIT_SELLER_SEND_GOODS'});
	//如果最近同步时间大于1小时的，自动同步一下
	if(lastSyncOrdersTime==null || lastSyncOrdersTime=='' || (Math.abs((new Date(Date.parse(lastSyncOrdersTime.replace(/-/g,   "/"))).getTime() - new Date()))/(1000*60*60))>=1){
		needAuth = false;
		$('#syncOrdersBut').click();
		needAuth = true;
	}
});

$.views.converters({
    displayInt : function(a,b){
    	if(a==null || a=='')a=0;
    	if(b==null || b=='')b=0;
		return Number(a)+Number(b);
	},
	displayDouble : function(a,b){
    	if(a==null || a=='')a=0.00;
    	if(b==null || b=='')b=0.00;
		return parseFloat(a)+parseFloat(b);
	}
});

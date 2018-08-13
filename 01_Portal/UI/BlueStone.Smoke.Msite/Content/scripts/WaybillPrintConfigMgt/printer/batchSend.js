var Order = Backbone.Model.extend({
	url:'/printer/printerAction!tradeDeal.action',
	initialize : function(){
	
	}
});
var OrdersList =  Backbone.Collection.extend({
	model : Order,
	url:'/printer/printerAction!batchSend.action?isDefault=no&showSendInfo=1'
});
var Orders = new OrdersList();
var OrdersRowView = MyView.extend({
	tagName : 'tr',
	template:$("#ordersRow-template" ),
	initialize : function(){
		this.model.bind('change',this.render,this);//model发生改变,便重新渲染
	},
	events :{
		"click .viewOrderDest" : "viewOrderDestHandler",
		"click .sendGoods" : "sendGoodsHandler",
		"click .isShowOrderDest" : "isShowOrderDestHandler",
		"click .editReceiverInfo" : "editReceiverInfoHandler"
	},
	render: function() {
		var html=this.template.render(this.model.toJSON());
	    $(this.el).html(html);
	    $('a[rel="popover"]').popover();
	    return this;
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
    },
    sendGoodsHandler : function(e){
    	if('taobao'==parent.domainId){
			try{
				if(!TaobaoShortAuth(parent.appKey,parent.ServerName,parent.nick)){
					return false;
				}
			}catch(err){alert(err)}
		}
    	var thisObj = this;
    	if(this.model.get('tradeStatus')=='WAIT_BUYER_CONFIRM_GOODS'){
    		var win = layer.confirm('订单已发货，是否重复发货？',function(){
    			layer.close(win);
    			_doSendGoods();
    		})
    	}else{
    		_doSendGoods();
    	}
    	function _doSendGoods(){
    		if(appV.checkTrackNumber(thisObj.model.get('tid'))){
	    		var param = {};
	    		param.type = 'sendGoods';
	    		var tid = thisObj.model.get('tid');
	    		var sends = [{tid:tid,trackNumber:$('#'+tid+'_trackNumber').val(),expressCompnanyForSend:PrinterUtil.getRealExpressCompnany($('#'+tid+'_expressCompnanyForSend').val())}];
	    		param.sends = JSON.stringify(sends);
	    		var model = thisObj.model;
	    		model.fetch({param:param,beforeSend:function(){$(e.currentTarget).button('loading')},success:function(model,data){
			    	var result = eval('('+data+')');
			    	if(result.result!='error'){
			    		sends = result.result; 
			    		model.set(sends[0]);
			    	}else{
			    		layer.alert(result.msg);
			    	}
		    	}})
	    	}
    	}
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
		"click #batchSend" : "batchSendHandler",
		"click #chooseAll" : "chooseAllHandler"
	},
	render: function() {
		var html=this.template.render(this.config);
		
	    $(this.el).html(html);
	    var arr = this.model.models;
	    var j=1;
	    for(var i=0;i<arr.length;i++){
	    	if(arr[i]!=null && arr[i].get('tid')!=null){
	    		arr[i].set({index:j,expressCompnanyDatas:expressCompnany_datas,lcs:lcs});
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
		table.append(view.el);
		return view;
	},
	hide : function(){
		$(this.el).css('display','none');
	},
	show : function(){
		$(this.el).css('display','block');
	},
	batchSendHandler : function(e){
		if('taobao'==parent.domainId){
			try{
				if(!TaobaoShortAuth(parent.appKey,parent.ServerName,parent.nick)){
					return false;
				}
			}catch(err){alert(err)}
		}
		var thisObj = this;
		var tid = appV.findCheckedTids();
		if(tid==''){
			layer.alert('请选择要发货的订单');
			return;
		}
		//是否已发货
		var tempModelArr = Orders.models;
		var hadSended = 0;
		for(var i=0;i<tempModelArr.length;i++){
			if((','+tid+',').indexOf(','+tempModelArr[i].get('tid')+',')!=-1){
				
				if('WAIT_BUYER_CONFIRM_GOODS'==tempModelArr[i].get('tradeStatus')){
					hadSended++;
				}
			}
		}
		if(hadSended>0){
			var win = layer.confirm('有'+hadSended+'个订单已发货，是否重复发货？',function(){
				layer.close(win);
				 _doSend();
			})
		}else{
			 _doSend();
		}
		//
		function _doSend(){
			var tidArr = tid.split(',');
			var sends = [];
			for(var i=0;i<tidArr.length;i++){
				if(tidArr[i]!=null && tidArr[i]!=''){
					if(appV.checkTrackNumber(tidArr[i])){
						sends.push({tid:tidArr[i],trackNumber:$('#'+tidArr[i]+'_trackNumber').val(),expressCompnanyForSend:PrinterUtil.getRealExpressCompnany($('#'+tidArr[i]+'_expressCompnanyForSend').val())})
					}else{
						return;
					}
				}
			}
			if(sends.length>0){
				var tempModelArr = [];
				var arr = thisObj.model.models;
				for(var k=0;k<arr.length;k++){
					tempModelArr.push(arr[k]);
				}
				var param = {};
	    		param.type = 'batchSendGoods';
	    		param.sends = JSON.stringify(sends);
	    		thisObj.model.fetch({param:param,beforeSend:function(){$(e.currentTarget).button('loading')},success:function(model,data){
			    	var result = eval('('+data+')');
			    	if(result.result!='error'){
			    		var sends = result.result;
			    		for(i=0;i<sends.length;i++){
			    			for(var a=0;a<tempModelArr.length;a++){
			    				if(sends[i].tid==tempModelArr[a].get('tid')){
			    					sends[i].checked = true;
			    					tempModelArr[a].set(sends[i]);
			    					break;
			    				}
			    			}
			    		}
			    	}else{
			    		layer.alert(result.msg);
			    	}
			    	$(e.currentTarget).button('reset')
		    	}})
			}
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
		"click #searchBut" : "search",
		"click #resetBut" : "resetHandler"
	},
	initialize: function() {
		
	},
	syncOrdersHandler : function(e){
		if('taobao'==parent.domainId){
			try{
				if(!TaobaoShortAuth(parent.appKey,parent.ServerName,parent.nick)){
					return false;
				}
			}catch(err){alert(err)}
		}
		Orders.fetch({url:'/printer/printerAction!printExpressPage.action?isDefault=no',param:{type:'syncOrders',isAutoMergeOrders:($('#isAutoMergeOrders').is(':checked')?"1":"0"),lastSyncOrdersTime:lastSyncOrdersTime},beforeSend:function(){$(e.currentTarget).button('loading')},
			success:function(model,data){
		    	var result = eval('('+data+')');
		    	$(e.currentTarget).button('reset')
		    	if(result.result=='success'){
		    		$('#syncTip').html($('#syncTip-template').render(result.baseInfo));
		    		baseInfo = result.baseInfo;
		    		lastSyncOrdersTime = result.baseInfo.lastSyncOrdersTime;
		    		appV.listV.config.loadingOrders = true;
		    		appV.showOrdersList(appV.listV.config);
		    	}else{
		    		layer.alert(result.msg);
		    	}
	    	}});
	},
	doSyncOrder : function(e){
		
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
		    			var order = new Order();
		    			order.set(list[i]);
		    			Orders.add(order);
		    		}
		    		config.totalPage=result.totalPage;
		    		config.totalRows=result.totalRows;
		    		config.currentPage=result.currentPage;
		    		lcs = result.lcs;
		    		config.loadingOrders=false;
		    		config.simplePage = simplePage;
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
		appV.listV.config.operateType = $('#operateType').val();
		appV.listV.config.sku=$('#sku').val();
		
		appV.listV.config.loadingOrders=true;
    	appV.showOrdersList(appV.listV.config);
	},
	resetHandler : function(e){
		$('#searchConfig').find('input[type="text"]').val('');
		$('#searchConfig').find('select').val('');
		$('#searchConfig').find('input[type="radio"]').removeAttr('checked');
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
	checkTrackNumber : function(tid){
		var number = $('#'+tid+'_trackNumber').val();
		if(number!=''){
			 
		}else{
			showToolTip($('#'+tid+'_trackNumber'),'运单号不能为空');
			return false;
		}
		var c = $('#'+tid+'_expressCompnanyForSend').val();
		if(c==null || c==''){
			showToolTip($('#'+tid+'_expressCompnanyForSend'),'物流公司不能为空');
			return false;
		}
		return true;
	}
});
var appV;
var lcs = null;
if(parent.displayLoadingBar!=null)parent.displayLoadingBar(null,null,'hide');
$(document).ready(function(){
	if('1'!=simplePage){
		$('#syncTip').html($('#syncTip-template').render(baseInfo));
	}
	appV = new AppView();
	if(tids!=null && tids!=''){
		appV.showOrdersList({tids:tids});
	}else{
		appV.showOrdersList({tradeStatus:'WAIT_SELLER_SEND_GOODS'});
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

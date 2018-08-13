var g1,g2,g3,g4,g5,g6;
$(document).ready(function(){
	loadData();
	g1 = new JustGage({
          id: "g1", 
          value: 0, 
          min: 0,
          max: 100,
          title: "打快递单率",
          label: "%",
          levelColorsGradient: false
        });
        
   g2 = new JustGage({
          id: "g2", 
          value: 0,
          min: 0,
          max: 100,
          title: "打快递单及时率",
          label: "%",
          levelColorsGradient: false
        });
        
   g3 = new JustGage({
          id: "g3", 
          value: 0, 
          min: 0,
          max: 100,
          title: "发货率",
          label: "%",
          levelColorsGradient: false
        });
   g4 = new JustGage({
          id: "g4", 
          value: 0, 
          min: 0,
          max: 100,
          title: "发货及时率",
          label: "%",
          levelColorsGradient: false
        });
   g5 = new JustGage({
          id: "g5", 
          value: 0, 
          min: 0,
          max: 100,
          title: "回评率",
          label: "%",
          levelColorsGradient: false
        });
   g6 = new JustGage({
          id: "g6", 
          value: 0, 
          min: 0,
          max: 100,
          title: "关怀率",
          label: "%",
          levelColorsGradient: false
        });
   showSync();
})
function showSync(){
	$('#syncTip').html($('#syncTip-template').render(baseInfo));
	$('#syncOrdersBut').click(function(){
		if('taobao'==parent.domainId){
			try{
				if(!TaobaoShortAuth(parent.appKey,parent.ServerName,parent.nick)){
					return false;
				}
			}catch(err){alert(err)}
		}
   		var but = $(this);
   		$.ajaxq('syncOrders',{
				url:'/printer/printerAction!printExpressPage.action?isDefault=no&type=syncOrders',
				type:'POST',
				data:{lastSyncOrdersTime:lastSyncOrdersTime},
				dataType:'text',
				cache:false,
				async:true,
				timeout:30000,
				beforeSend: function(){
					but.button('loading');
			    },
			    complete: function(){
			    	but.button('reset');
			    },
			    success: function(data){
			    	var resultJson=eval('('+data+')');
					if(resultJson.result=="error"){
						layer.alert(resultJson.msg);
					}else{
						baseInfo = resultJson.baseInfo;
						lastSyncOrdersTime = resultJson.baseInfo.lastSyncOrdersTime;
						showSync();
						loadData();
					}
				}
			});
   })
}
function loadData(){
	$.ajaxq('loadData',{
		url:'/printer/printerAction!index.action?isDefault=no',
		type:'POST',
		data:{},
		dataType:'text',
		cache:false,
		async:true,
		timeout:30000,
		beforeSend: function(){
			parent.displayLoadingBar(null,null,'正在统计，请稍候...');
	    },
	    complete: function(){
	    	parent.displayLoadingBar(null,null,'hide');
	    },
	    success: function(data){
	    	var resultJson=eval('('+data+')');
			if(resultJson.result=="error"){
				layer.alert(resultJson.msg);
			}else{
				$('a').each(function(){
					var dataType = $(this).attr('data-type');
					if(dataType!=null && dataType!=''){
						$(this).html(resultJson[dataType]);
					}
				});
				if(resultJson.orderNum!=0)g1.refresh((resultJson.printed/resultJson.orderNum).toFixed(2)*100);
          		if(resultJson.printed!=0)g2.refresh((resultJson.onTimePrint/resultJson.printed).toFixed(2)*100);
          		if(resultJson.printed!=0)g3.refresh((resultJson.sended/resultJson.printed).toFixed(2)*100);
          		if(resultJson.sended!=0)g4.refresh((resultJson.onTimeSend/resultJson.sended).toFixed(2)*100);
			}
		}
	});
}
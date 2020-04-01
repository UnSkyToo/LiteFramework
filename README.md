# LiteFramework
LiteFramework For Unity3D  
Version 20.04.01.1  

Demo Unity Project 
https://github.com/UnSkyToo/LiteMore

UI Auto Binder  

1.  
[LiteUINode("xxx")]  
Transform Node;  
or  
Node = FindChild("xxx");  
  
2.  
[LiteUIComponent(xxx)]  
Image Node;  
or  
Node = GetComponent<Image>("xxx");  
  
3.  
[LiteUIEvent(xxx, event type)]  
void OnClick(){}  
or  
AddEventToChild(xxx, OnClick);  

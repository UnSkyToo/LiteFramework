local LogoUI = BaseClass('LogoUI', UIBase)

function LogoUI:Ctor()
	print('LogoUI Ctor')
end

function LogoUI:Dtor()
	print('LogoUI Dtor')
end

function LogoUI:OnOpen()
	print('LogoUI OnOpen')

	--local a = self._CSEntity_:FindChild('Tips/Value'):GetComponent(typeof(CS.UnityEngine.UI.Text))
	--a.text = "adsadasd"

	self:FindChild('11111111111111')
end

function LogoUI:OnClose()
	print('LogoUI OnClose')
end

return LogoUI
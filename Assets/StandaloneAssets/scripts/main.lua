require 'framework.init'

local main = {}

function main:OnStart()
	print('main start')
	self.lo = UIManager:OpenUI('logoui', 'logoui')
	return true
end

function main:OnStop()
	UIManager:CloseUI(self.lo)
	print('main stop')
end

function main:OnTick(dt)
	--print('main tick ' .. dt)
end

function main:OnEnterForeground()
	print('OnEnterForeground')
end

function main:OnEnterBackground()
	print('OnEnterBackground')
end

print('lite framework startup')
return main
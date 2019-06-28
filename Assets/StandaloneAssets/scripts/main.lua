local main = {}

function main:OnStart()
	print('main start')
	return true
end

function main:OnStop()
	print('main stop')
end

function main:OnTick(dt)
	print('main tick ' .. dt)
end

print('lite framework startup')
return main
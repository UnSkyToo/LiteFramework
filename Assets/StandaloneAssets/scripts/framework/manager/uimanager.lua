UIManager = {}

local openUI_ = CS.Lite.Framework.Lua.LuaRuntime.OpenLuaUI
local closeUI_ = CS.Lite.Framework.Lua.LuaRuntime.CloseLuaUI

function UIManager:OpenUI(uipath, prefabpath, ...)
	local ui = require('logic.ui.' .. uipath):Create(...)
	openUI_(prefabpath, ui)
	return ui
end

function UIManager:CloseUI(ui)
	ui:Delete()
	closeUI_(ui)
end
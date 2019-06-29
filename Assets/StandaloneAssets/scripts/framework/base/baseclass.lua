function BindHandler(obj, method)
    return function(...)
        return method(obj, ...)
    end
end

local setmetatableindex_
setmetatableindex_ = function(t, index)
    if type(t) == "userdata" then
        local peer = tolua.getpeer(t)
        if not peer then
            peer = {}
            tolua.setpeer(t, peer)
        end
        setmetatableindex_(peer, index)
    else
        local mt = getmetatable(t)
        if not mt then
            mt = {} 
        end
        
        if not mt.__index then
            mt.__index = index
            setmetatable(t, mt)
        elseif mt.__index ~= index then
        
            local ttt = {}
            setmetatable(ttt,index)
        
            setmetatableindex_(mt, index)
        end
    end
end
setmetatableindex = setmetatableindex_

function BaseClass(classname, ...)
    local cls = {__cname = classname}

    local supers = {...}
    for _, super in ipairs(supers) do
        local superType = type(super)
        assert(superType == "nil" or superType == "table" or superType == "function",
            string.format("BaseClass() - create class \"%s\" with invalid super class type \"%s\"",
                classname, superType))

        if superType == "function" then
            assert(cls.__create == nil,
                string.format("BaseClass() - create class \"%s\" with more than one creating function",
                    classname));
            -- if super is function, set it to __create
            cls.__create = super
        elseif superType == "table" then
            -- super is pure lua class
            cls.__supers = cls.__supers or {}
            cls.__supers[#cls.__supers + 1] = super
            if not cls.super then
                -- set first super pure lua class as class.super
                cls.super = super
            end
        else
            error(string.format("BaseClass() - create class \"%s\" with invalid super type",
                        classname), 0)
        end
    end

    cls.__index = cls

    if not cls.__supers or #cls.__supers == 1 then
        setmetatable(cls, {__index = cls.super})
    else
        setmetatable(cls, {__index = function(_, key)
            local supers = cls.__supers
            for i = 1, #supers do
                local super = supers[i]
                if super[key] then return super[key] end
            end
        end})
    end

    if not cls.Ctor then
        -- add default constructor
        cls.Ctor = function() end
    end
    cls.New = function(...)
        local instance
        if cls.__create then
            instance = cls.__create(...)
        else
            instance = {}
        end
        
        setmetatableindex(instance, cls)
        instance.class = cls
        cls.realize = instance
        instance:Ctor(...)
        
        return instance
    end
    cls.Del = function()
        if not cls.realize then
            return
        end

        cls.realize:Dtor()
    end

    cls.Create = function(_, ...)
        return cls.New(...)
    end
    cls.Delete = function(_)
        cls.Del()
    end

    return cls
end
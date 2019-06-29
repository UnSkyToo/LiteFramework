function BindHandler(obj, method)
    return function(...)
        return method(obj, ...)
    end
end

function BaseClass(classname, base)  
    local cls = {}  
    if base then
        setmetatable(cls, {__index = base})
        cls.base = base
    end
  
    cls.__cname = classname  
    cls.__index = cls

    cls.New = function(...)
        local instance = setmetatable({}, cls)
        local create
        create = function(c, ...)
             if c.base then
                  create(c.base, ...)
             end
             if c.Ctor then
                  c.Ctor(instance, ...)
             end
        end
        instance.class = cls
        cls.realize = instance
        create(instance, ...)
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
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
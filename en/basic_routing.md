# Basic routing

It's time to extend our WebPart to support multiple routes.
First, let's extract the WebPart and bind it to an identifier:

==> App.fs:`let webPart`

In C#, one would call it "assign webPart to a variable", but in functional world there's really no concept of a variable. Instead, we can "bind" a value to an identifier, which we can reuse later.
Values, once bound, can't be mutated during runtime.
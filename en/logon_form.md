# Logon form

There's no handler for the `logon` route yet, so we need to create one.
Logon view will be rather straightforward - just a simple form with username and password.

```fsharp
type Logon = {
    Username : string
    Password : Password
}

let logon : Form<Logon> = Form ([],[])
```

Above snippet shows how the `logon` form can be defined in our `Form` module.
`Password` is a type from Suave library and helps to determine the input type for HTML markup (we don't want anyone to see our secret pass as we type it).

```fsharp
let logon = [
    h2 "Log On"
    p [
        text "Please enter your user name and password."
    ]

    renderForm
        { Form = Form.logon
          Fieldsets = 
              [ { Legend = "Account Information"
                  Fields = 
                      [ { Label = "User Name"
                          Xml = input (fun f -> <@ f.Username @>) [] }
                        { Label = "Password"
                          Xml = input (fun f -> <@ f.Password @>) [] } ] } ]
          SubmitText = "Log On" }
]
```

As I promised, nothing fancy here.
We've already seen how the `renderForm` works, so the above snippet is just another plain HTML form with some additional instructions at the top.

The GET handler for `logon` is also very simple:

```fsharp
let logon =
    View.logon
    |> html
```

```fsharp
path Path.Account.logon >=> logon
```

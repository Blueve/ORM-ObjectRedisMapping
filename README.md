# ORM - Object Redis Mapping (Developing)

[![Build Status](https://dev.azure.com/blueveyoud/ORM-ObjectRedisMapping/_apis/build/status/Blueve.ORM-ObjectRedisMapping?branchName=master)](https://dev.azure.com/blueveyoud/ORM-ObjectRedisMapping/_build/latest?definitionId=1?branchName=master)
![](https://img.shields.io/azure-devops/tests/blueveyoud/ORM-ObjectRedisMapping/1/master.svg)
![](https://img.shields.io/azure-devops/coverage/blueveyoud/ORM-ObjectRedisMapping/1/master.svg)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/2a5b5ae1dfbe4ce4a7db402e572c49e9)](https://app.codacy.com/app/672454911/ORM-ObjectRedisMapping?utm_source=github.com&utm_medium=referral&utm_content=Blueve/ORM-ObjectRedisMapping&utm_campaign=Badge_Grade_Settings)


A tool to map and associate objects(entities) with Key-Value based database Redis.

You will no longer have to worry about how to design Redis schema and needn't to deal with data acuisition and writing too with this tool.

This is a toy project developed out of personal interest. I will try to use this project to experiment with some problemes in my work. Although I have no way to apply the technology used in this project in my work, I believe that the experience I gained in developing this project will be of great benefit to me.

If you are interested in this project too, you are welcome to put forward your suggestions in [Issues](https://github.com/Blueve/ORM-ObjectRedisMapping/issues) and you are also welcome to contribute this project with me.

---
SINCE THIS PROJECT IS UNDER DEVELOPING, THE SHOW CASE BELOW IS JUST FOR PREVIEW THE API AND THE USAGE.

## Show Case

Store to Redis
```csharp
var person1 = new Person
{
    Name = "Tom",
    Age = 18
};
var person2 = new Person
{
    Name = "Jerry",
    Age = 10
};
person1.Partner = person2;
person2.Partner = person1;

dbContext.Commit(person1);

// Redis
// PersonTom -> True
// PersonTomName -> Tom
// PersonTomeAge -> 18
// PersonTomPartner -> Jerry
// PersonJerry -> True
// PersonJerryName -> Jerry
// PersonJerryAge -> 10
// PersonJerryPartner -> Tom
```

Fetch from Redis
```csharp
var person = dbContext.Find("Tom");
Console.WriteLine(person.Partern.Name); // Jerry
Console.WriteLine(person.Partern.Age); // 10
Console.WriteLine(person.Partern.Partern.Name); // Tom
Console.WriteLine(person.Partern.Partern.Name); // 18
```

Update in place by a dynmic proxy
```csharp
var person = dbContext.Find("Jerry");
person.Age = 18;

// Redis
// PersonJerryAge -> 18
```

Store a linked-list
```csharp
var head = new ListNode
{
    Val = 1,
    Next = new ListNode
    {
        Val = 2,
        Next = new ListNode
        {
            Val = 3
        }
    }
};
dbContext.Commit(head);

// Redis
// ListNode1 -> True
// ListNode1Val -> 1
// ListNode1Next -> 2
// ListNode2 -> True
// ListNode2Val -> 2
// ListNode2Next -> 3
// ListNode3 -> True
// ListNode3Val -> 3
```
---

## Contribution Guide

// TODO

---

## Licence

MIT License

Copyright (c) 2019 Blueve

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
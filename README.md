# ORM - Object Redis Mapping (Developing)
---
A tool to map and associate objects(entities) with Key-Value based database Redis.

With this tool, you will no longer have to worry about hot to design Redis schema and data acuisition and writing complexity.

This is a toy project developed out of personal interest. I will try to use this project to experiment with some problemes in my work. Although I have no way to apply the technology used in this project in my work, I beliveve that the experience I gained in developing this project will be of great benefit to me.

If you are interested in this project too, you are welcome to put forward your suggestions in [Issue] and you are also welcome to contribute this project with me.

---
SINCE THIS PROJECT IS UNDER DEVELOPING, THE SHOW CASE BELOW IS JUST FOR PREVIEW THE API AND USAGE.

## Show Case

Store to Redis
```csharp
var person1 = new Person
{
    Name = "Tom"
};
var person2 = new Person
{
    Name = "Jerry"
};
person1.Partner = person2;
person2.Partner = person1;

dbContext.Commit(person1);
dbContext.Commit(person2);

// Redis
// Person00000003TomId -> Tom
// Person00000003TomName -> Tom
// Person00000003TomPartner -> Person00000005Jerry
// Person00000005JerryId -> Jerry
// Person00000005JerryName -> Jerry
// Person00000005JerryPartner -> Person00000003Tom
```

Fetch from Redis
```csharp
var person = dbContext.Find("Tom");
Console.WriteLine(person.Partern.Name); // Jerry
Console.WriteLine(person.Partern.Partern.Name); // Tom
```

Update in place by a dynmic proxy
```csharp
var person = dbContext.Find("Jerry");
person.Age = 18;

// Redis
// Person00000005JerryAge -> 18
```

## Licence

{MIT}
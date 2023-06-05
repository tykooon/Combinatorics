using Permutations;

var list = new List<string>()
{
    "Ananas",
    "Apple",
    "Banana",
    "Lemon",
    "Orange",
    "Peach",
    "Peer",
    "Watermelon"
};

var perm = Permutation.Random(list.Count);
var newList = perm.Apply(list);

Console.WriteLine(perm.ToString("cycle"));

foreach(var item in newList)
{
    Console.WriteLine(item);
}

Console.WriteLine();

perm = Permutation.Random(list.Count);
newList = perm.Apply(list);

Console.WriteLine(perm.ToString("cycle"));

foreach (var item in newList)
{
    Console.WriteLine(item);
}
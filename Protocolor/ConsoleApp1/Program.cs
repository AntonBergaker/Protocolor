using Protocolor.Parsing;
using Protocolor.Tokenization;
using Protocolor.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

Tokenizer tokenizer = new Tokenizer();

Image<Bgra32> image = Image.Load<Bgra32>(args[0]);

var (tokens, errors) = tokenizer.Tokenize(Utils.ImageToArray(image));

if (errors.Length > 0) {
    throw new Exception();
}

Parser parser = new Parser();
var (_, astErrors) = parser.Parse(tokens);

if (astErrors.Length > 0) {
    throw new Exception();
}
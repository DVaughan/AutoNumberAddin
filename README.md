# Auto Number Addin for Markdown Monster
This project is an addin for [Markdown Monster](https://markdownmonster.west-wind.com/). 
This addin automatically numbers figures, tables, and code listings.

For example, the addin automatically turns the following:

The method iterates 10 times. (See Listing 0.)

**Listing 0.** Some method1
```
for (var i = 0; i < 10; i++)
{
	...
}
```

Into this:

The method iterates 10 times. (See Listing 1.)

**Listing 1.** Some method1
```
for (var i = 0; i < 10; i++)
{
	...
}
```

Notice that the listing number changed.

Code listings, figures, and captions are all numbered sequentially.

## How it works
This addin uses conventions to determine what is a caption or a reference to a caption.

There are three pairs of regular expressions. Each pair consists of a regular expression for the caption e.g. ```**Lising 1.** Some method``` and a regular expression for the reference, e.g. ```See Listing 1.```

The following are the default pairs of regular expressions:
* ```^.*?See Table (?<Number>\d{1,3})\.?.*$```
* ```^\s*\*\*\s*Table (?<Number>\d{1,3})\.\s*\*\*.*$```
* ```^.*?See Figure (?<Number>\d{1,3})\.?.*$```
* ```.*?<figcaption>Figure (?<Number>\d{1,3})\..*```
* ```^.*?See Listing (?<Number>\d{1,3})\.?.*$```
* ```^\s*\*\*\s*Listing (?<Number>\d{1,3})\.\s*\*\*.*$```

Do not worry if you're befuddled by these regular expressions. You will only need to understand them if you wish to use different conventions in your markdown document.

Listing and Table captions are detected if they are surrounded by bold \** asterisks. There must be a single space between Listing caption type and the number.

> **NOTE:** You can modify the regular expressions to suit your needs in the addin's configuration.

### Ignoring Captions or Caption References
To have the addin ignore a caption, or caption reference, in your document place ```[//]: # (AutoNumberIgnore)``` somewhere before the caption or caption reference occurs. You can use multiple ignore markers to disable multiple captions or references.

### A note about figures
I like to use HTML for figures because it allows me to use the HTML5 *figcaption* element. This means that the addin will match the following:

```html
<figure><img src='https://avatars1.githubusercontent.com/u/26504010?v=4&s=200'><figcaption>Codon Framework</figcaption></figure>
```


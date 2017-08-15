<?php

$xml = <<<HERE
<?xml version="1.0" encoding="ISO-8859-1" ?>
<foo xmlns="http://www.example.com/ns/foo"
     xmlns:fubar="http://www.example.com/ns/fubar">
  <bar>Lorem Ipsum</bar>
  <!-- A comment -->
  <bar><test2><![CDATA[Within this Character Data block I can use --, <, &, ', and " as much as I want]]></test2></bar>
  <fubar:bar><test3 /></fubar:bar>
  <fubar:bar><test4 /></fubar:bar>
</foo>
HERE;

function dump($elems) {
	foreach ($elems as $elem) {
		echo $elem->nodeName ." ";

    if ($elem instanceof DOMCharacterData) {
      echo "'". $elem->data ."' ";
    }

    if ($elem->hasChildNodes()) {
      dump($elem->childNodes);
    }
	}
}

$dom = new DOMDocument();
$dom->loadXML($xml);
$doc = $dom->documentElement;
dump($dom->getElementsByTagName('bar'));
dump($doc->getElementsByTagName('bar'));
dump($dom->getElementsByTagNameNS('http://www.example.com/ns/fubar', 'bar'));
dump($doc->getElementsByTagNameNS('http://www.example.com/ns/fubar', 'bar'));
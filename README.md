# Ancora
Ancora is a recursive descent parser generator library.

Unlike most parser generators, Ancora does not produce a source file that implements your parser, nor does it produce a set of tables to control a shift-reduce parser. Ancora builds a recursive descent parser out of generic pieces without having to invoke another language, or add another build step to generate the parser source. Recursive descent parsers are fast, effecient, and far more powerful than other parsers. There's no need to worry about look ahead; Ancora can look ahead as far as your grammar requires.



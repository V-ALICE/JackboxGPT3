# JackboxGPT3

Because we wanted to use AI for party games instead of "useful" things.

This project is a Jackbox client controlled by GPT-3 (note: this is a pre-ChatGPT model). It currently supports these games:

- Fibbage XL/2/3/4
- Quiplash XL/2/3
- Blather 'Round
- Joke Boat
- Survive the Internet _(currently chooses images/votes randomly)_
- Word Spud _(currently always votes positively)_

## Playing

For now the only way to run JackboxGPT3 is to build it yourself. You'll also need to provide an OpenAI API key as an environment variable, either set or in a `.env` file, named `OPENAI_API_KEY`.

To play a game, simply run the compiled executable and enter "Number of Instances" and "Room Code" when prompted. The executable can also be run with command line args as input, run with the `--help` option to see usage information.

## Adding Support for More Games

See [this guide](Extending.md) for some information on adding more games.

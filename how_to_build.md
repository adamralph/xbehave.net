# How to build

These instructions are *only* for building with Rake, which includes compilation, test execution and packaging. This is the simplest way to build.

You can also build the solution using Visual Studio 2012 or later.

*Don't be put off by the prerequisites!* It only takes a few minutes to set them up and only needs to be done once. If you haven't used [Rake](http://rake.rubyforge.org/ "RAKE -- Ruby Make") before then you're in for a real treat!

At the time of writing the build is only confirmed to work on Windows using the Microsoft .NET framework.

## Prerequisites

1. Ensure you have .NET framework 3.5 and 4.5 installed.

1. Install Ruby 1.8.7 or later.

 For Windows we recommend using [RubyInstaller](http://rubyinstaller.org/) and selecting 'Add Ruby executables to your PATH' when prompted. For alternatives see the [Ruby download page](http://www.ruby-lang.org/en/downloads/).
1. Using a command prompt, update RubyGems to the latest version:

    `gem update --system`

1. Install Bundler:

    `gem install bundler`

1. Install the required gems:

    `bundle install`

## Building

Using a command prompt, navigate to your clone root folder and execute:

`build.cmd`

This executes the default build tasks. After the build has completed, the build artifacts will be located in `artifacts/output/`.

## Extras

* View the full list of build tasks:

    `build.cmd -T`

* Run a specific task:

    `build.cmd spec`

* Run multiple tasks:

    `build.cmd spec pack`

* View the full list of rake options:

    `build.cmd -h`

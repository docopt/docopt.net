# Documentation

This documentation site is built using [MkDocs] and [mkdocs-material]. These
tools generate a static website based on a configuration file and a set of
Markdown files in [the `doc/main` branch][branch].

  [MkDocs]: https://www.mkdocs.org/
  [mkdocs-material]: https://squidfunk.github.io/mkdocs-material/
  [branch]: https://github.com/docopt/docopt.net/tree/doc/main

## Working locally

Supposing you have the Git repository cloned, checkout the branch that
contains the documentation:

    git checkout doc/main

The simplest approach is to use Docker to build and serve the static site
locally.

Build the image using:

    docker build -t docopt-net-doc .

Once successfully built, run the image using:

    docker run --rm -it -p 8000:8000 -v ${PWD}:/docs docopt-net-doc

Open a browser and navigate to `http://localhost:8000/`.

## Configuration

The file `mkdocs.yml` provides the main configuration for the website, such as
color and themes, plugins and extension. The table of contents is also defined
in the configuration file, under the section `nav`. It requires a manual
update when a new page is added to the documentation.

## Versioning

Each version has its own documentation. When a version is released, its
documentation is archived under a directory with a `v` prefix, as in `v0.7`.
The `archive.sh` script can be used to perform the archival:

    ./archive.sh <version>

Replace `<version>` with a version number like `0.7` or `1.x`.

!!! note

    Some files in the documentation are symbolic links because their content
    does not change across versions. After running the archival script, check
    that symbolic links were preserved and fix as necessary.

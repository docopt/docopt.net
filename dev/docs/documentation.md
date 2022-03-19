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

    docker run --rm -it -p 8000:8000 -v ${PWD}:/docs -w /docs/dev docopt-net-doc

Open a browser and navigate to `http://localhost:8000/`.

## Configuration

The file `mkdocs.yml` provides the main configuration for the website, such as
color and themes, plugins and extension. The table of contents is also defined
in the configuration file, under the section `nav`. It requires a manual
update when a new page is added to the documentation.

## Versioning

Each version has its own documentation. When a version is released, its
documentation is archived under a directory with a `v` prefix, as in `v0.7`.
The latest documentation that is currently under development is under `dev`.

The archival steps are as follows:

1. Copy all versioned files and directories under `dev` to an adjacent
   directory for the new version.

2. Edit `mkdocs.yml` of the archived version and update the URL for `site_url`
   and `edit_uri` such that `dev` is replaced with the directory name of the
   archived version.

3. Add the archived version's directory to version control (Git).

The container setup described in the [Working Locally] section can also be
used to build the complete documentation with all versions using:
    
    docker run --rm -v ${PWD}:/docs --entrypoint /bin/sh docopt-net-doc ./build.sh

[Working Locally]: #working-locally

!!! note

    Some files in the documentation are symbolic links because their content
    does not change across versions. Check the symbolic links were preserved and fix as necessary.

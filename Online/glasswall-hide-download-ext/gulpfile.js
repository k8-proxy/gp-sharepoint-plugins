'use strict';

// check if gulp dist was called
if (process.argv.indexOf('dist') !== -1) {
    // add ship options to command call
    process.argv.push('--ship');
}

const build = require('@microsoft/sp-build-web');

build.addSuppression(`Warning - [sass] The local CSS class 'ms-Grid' is not camelCase and will not be type-safe.`);

// add warning suppression for immediate deployment
build.addSuppression(/Warning - Admins can make this solution available to all sites immediately/gi);

// Create clean dist package
const gulp = require('gulp');
const gulpSequence = require('gulp-sequence');

gulp.task('dist', gulpSequence('clean', 'bundle', 'package-solution'));

build.initialize(require('gulp'));

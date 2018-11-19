// var [base-ref-directory-changer = require('a task that changes the Angular base ref depending upon which ENVIRONMENT the code is in')]
var autoprefixer = require('gulp-autoprefixer');
// var browserSync = require('browser-sync').create();
var cssmin = require('gulp-cssmin');
var concat = require('gulp-concat');
var del = require('del');   //deletes unnecessary files in the dest folder
var errorHandler = require('gulp-error-handle');
var gulp = require('gulp');
var gutil = require('gulp-util');
var jshint = require('gulp-jshint');
// var livereload = require('gulp-livereload');
var nib = require('nib');   //Stylus mixins, utilities, components, and gradient image generation.
var rename = require('gulp-rename');    //Renames minified files for better performance
var runSequence = require('run-sequence');
var sass = require('gulp-sass');
var stripCss = require('gulp-strip-css-comments');
var stripDebug = require('gulp-strip-debug');
var stylus = require('gulp-stylus');
var uglify = require('gulp-uglify');
var vinylPaths = require('vinyl-paths');    //Useful when you need to use the file paths from a gulp pipeline in Promise-returning node module.

var paths = {
	js: 'src/js/*.js',
	jsConcatSrc: ['!src/js/scripts.concat.js', 'src/js/*.js'],
	jsDist: 'dist/js',
	jsSrc: 'src/js',
	jsConcatFilename: 'scripts.concat.js',
	jsMinFilename: 'scripts.min.js',
	//  vendorJsMinFilename: 'vendor.min.js',
	css: 'css',
	cssDist: 'dist/css',
	cssSrc: 'src/css',
	cssWatch: 'src/styl/**/*.styl',
	cssWatch2: 'src/styl/**/**/*.styl',
	cssMinFilename: 'main.min.css',
	cssConcatFilename: 'main.concat.css',
	cssStylFilename: '_main-styl.scss',
	//fonts: 'src/fonts/**/*',
	//fontsSrc: 'src/css/fonts',
	mainStyl: 'src/styl/main.styl'//,
	//  jqueryFilepath: '../node_modules/jquery/dist/jquery.min.js',
	//  jqueryMinFilename: 'jquery.min.js'
};

// var jsVendorFileList = [
//     '../node_modules/slick-carousel/slick/slick.js',
//     '../node_modules/remodal/dist/remodal.js',
//     '../node_modules/sumoselect/jquery.sumoselect.js'
// ];

gulp.task('default', function (callback) {
	runSequence('clean', 'styles', 'js', /*'browser-sync',*/'watch:styles', 'watch:js', callback);
});

gulp.task('build', function (callback) {
	runSequence('clean', 'styles', 'js', callback);
});

gulp.task('clean', function (callback) {
	runSequence('clean:dist', 'clean:src', 'clean:js', callback);
});

gulp.task('styles', function (callback) {
	runSequence('stylus', 'sass', 'prefix', callback);
});

gulp.task('js', function (callback) {
	runSequence('lint', 'scripts:main', /*'scripts:vendor', 'scripts:jquery',*/ callback);
});

gulp.task('watch:styles', function () {
	// livereload.listen();
	gulp.watch([paths.cssWatch, paths.cssWatch2], ['styles']);
});
gulp.task('watch:js', function () {
	// livereload.listen();
	gulp.watch(paths.js, ['lint', 'scripts:main']);
});

//gulp.task('copy:fonts', function () {
//	gulp.src([paths.fonts]).pipe(gulp.dest(paths.fontsSrc));
//});

gulp.task('stylus', function () {
	return gulp.src(paths.mainStyl)
		.pipe(stylus({
			import: ['nib'],
			use: [nib()],
			'include css': true
		}))
		.pipe(errorHandler())
		.pipe(rename(paths.cssStylFilename))
		.pipe(gulp.dest(paths.cssSrc));
	// .pipe(livereload());
});

gulp.task('sass', function () {
	return gulp.src('src/sass/main.scss')
		.pipe(sass())
		.pipe(stripCss())
		.pipe(rename(paths.cssConcatFilename))
		.pipe(gulp.dest(paths.cssSrc))
		.pipe(cssmin())
		.pipe(rename(paths.cssMinFilename))
		.pipe(gulp.dest(paths.cssDist));
	// .pipe(livereload());
});

gulp.task('prefix', function () {
	return gulp.src('src/css/main.concat.css')
		.pipe(autoprefixer({
			browsers: ['last 2 versions'],
			cascade: false
		}))
		.pipe(rename(paths.cssConcatFilename))
		.pipe(gulp.dest(paths.cssSrc))
		.pipe(cssmin())
		.pipe(rename(paths.cssMinFilename))
		.pipe(gulp.dest(paths.cssDist));
	// .pipe(livereload());
});

// gulp.task('browser-sync', function() {
// 	browserSync.init({
// 		proxy: "localhost:80/pages/home.php",
// 		browser: 'chrome',
// 	});
// });

gulp.task('serve', ['js'], function () {
	gulp.watch("js/*.js", ['js-watch']);
});

gulp.task('lint', function () {
	return gulp.src(['gulpfile.js', paths.js])
		.pipe(jshint('.jshintrc'))
		.pipe(jshint.reporter('default'));
});

gulp.task('scripts:main', function () {
	return gulp.src(paths.jsConcatSrc)
		.pipe(concat(paths.jsConcatFilename))
		.pipe(gulp.dest(paths.jsSrc))
		.pipe(stripDebug())
		.pipe(uglify())
		.pipe(rename(paths.jsMinFilename))
		.pipe(gulp.dest(paths.jsDist));
});

// gulp.task('scripts:vendor', function() {
//   return gulp.src(jsVendorFileList)
//     // Concatenate everything
//     .pipe(concat(paths.jsConcatFilename))
//     // Strip all debugger code out.
//     .pipe(stripDebug())
//     // Minify the JavaScript.
//     .pipe(uglify())
//     .pipe(rename(paths.vendorJsMinFilename))
//     .pipe(gulp.dest(paths.jsDist +'/vendor/'));
// });

// gulp.task('scripts:jquery', function() {
//   return gulp.src(paths.jqueryFilepath)
//     // Strip all debugger code out.
//     //.pipe(stripDebug())
//     // Minify the JavaScript.
//     //.pipe(uglify())
//     .pipe(rename(paths.jqueryMinFilename))
//     .pipe(gulp.dest(paths.jsDist +'/vendor/'));
// });

gulp.task('clean:dist', function () {
	return gulp.src('dist/*')
		.pipe(vinylPaths(del))
		.pipe(stripDebug())
		.pipe(gulp.dest('dist'));
});
gulp.task('clean:src', function () {
	return gulp.src(paths.cssSrc + "/" + paths.cssStylFilename)
		.pipe(vinylPaths(del))
		.pipe(stripDebug())
		.pipe(gulp.dest('src'));
});
gulp.task('clean:js', function () {
	return gulp.src(paths.jsSrc + "/" + paths.jsConcatFilename)
		.pipe(vinylPaths(del))
		.pipe(stripDebug())
		.pipe(gulp.dest('src'));
});